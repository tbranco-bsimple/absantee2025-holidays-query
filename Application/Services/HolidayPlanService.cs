using Domain.IRepository;
using Domain.Interfaces;
using Domain.Models;
using Application.DTO;
using AutoMapper;
using Domain.Factory;
using Infrastructure.DataModel;

namespace Application.Services;

public class HolidayPlanService
{
    private readonly IHolidayPlanRepository _holidayPlanRepository;
    private readonly IHolidayPlanFactory _holidayPlanFactory;
    private readonly IHolidayPeriodFactory _holidayPeriodFactory;
    private readonly ICollaboratorRepository _collaboratorRepository;
    private readonly IAssociationProjectCollaboratorRepository _associationProjectCollaboratorRepository;
    private readonly IMapper _mapper;

    public HolidayPlanService(IHolidayPlanRepository holidayPlanRepository, IHolidayPlanFactory holidayPlanFactory, IHolidayPeriodFactory holidayPeriodFactory, ICollaboratorRepository collaboratorRepository, IAssociationProjectCollaboratorRepository associationProjectCollaboratorRepository, IMapper mapper)
    {
        _holidayPlanRepository = holidayPlanRepository;
        _holidayPlanFactory = holidayPlanFactory;
        _holidayPeriodFactory = holidayPeriodFactory;
        _collaboratorRepository = collaboratorRepository;
        _associationProjectCollaboratorRepository = associationProjectCollaboratorRepository;
        _mapper = mapper;
    }

    public async Task SubmitHolidayPlanAsync(Guid id, Guid collabId, List<HolidayPeriod> holidayPeriods)
    {
        var holidayPeriodsDataModel = _mapper.Map<List<HolidayPeriod>, List<HolidayPeriodDataModel>>(holidayPeriods);
        var visitor = new HolidayPlanDataModel()
        {
            Id = id,
            CollaboratorId = collabId,
            HolidayPeriods = holidayPeriodsDataModel
        };

        var holidayPlan = _holidayPlanFactory.Create(visitor);

        await _holidayPlanRepository.AddAsync(holidayPlan);
    }

    public async Task SubmitHolidayPeriodAsync(Guid holidayPlanId, Guid id, PeriodDate periodDate)
    {
        var visitor = new HolidayPeriodDataModel()
        {
            Id = id,
            PeriodDate = periodDate
        };

        var holidayPeriod = _holidayPeriodFactory.Create(visitor);

        await _holidayPlanRepository.AddHolidayPeriodAsync(holidayPlanId, holidayPeriod);
        await _holidayPlanRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<HolidayPeriodDTO>> FindHolidayPeriodForCollaborator(Guid collaboratorId)
    {
        var holidayPeriods = await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorAsync(collaboratorId);

        return holidayPeriods.Select(_mapper.Map<HolidayPeriod, HolidayPeriodDTO>);
    }

    //UC13
    public async Task<IEnumerable<HolidayPeriodDTO>> FindHolidayPeriodsByCollaboratorBetweenDatesAsync(Guid collaboratorId, PeriodDate periodDate)
    {
        var result = await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorBetweenDatesAsync(collaboratorId, periodDate);

        return result.Select(_mapper.Map<HolidayPeriod, HolidayPeriodDTO>);
    }

    // US14 - Como gestor de RH, quero listar os collaboradores que têm de férias num período
    public async Task<Result<IEnumerable<CollaboratorDTO>>> FindAllCollaboratorsWithHolidayPeriodsBetweenDates(PeriodDate periodDate)
    {
        try
        {
            var holidayPlans = await _holidayPlanRepository.FindHolidayPlansWithinPeriodAsync(periodDate);

            var collabIds = holidayPlans.Select(holidayPlans => holidayPlans.CollaboratorId);
            var collabs = await _collaboratorRepository.GetByIdsAsync(collabIds);

            return Result<IEnumerable<CollaboratorDTO>>.Success(collabs.Select(_mapper.Map<CollaboratorDTO>));
        }
        catch (Exception e)
        {
            return Result<IEnumerable<CollaboratorDTO>>.Failure(Error.InternalServerError(e.Message));
        }
    }

    //UC15: Como gestor de RH, quero listar os colaboradores que já registaram períodos de férias superiores a x dias 
    public async Task<Result<IEnumerable<CollaboratorDTO>>> FindAllWithHolidayPeriodsLongerThan(int days)
    {
        try
        {
            var holidayPlans = await _holidayPlanRepository.FindAllWithHolidayPeriodsLongerThanAsync(days);
            var collabIds = holidayPlans.Select(hp => hp.CollaboratorId);
            var collabs = await _collaboratorRepository.GetByIdsAsync(collabIds);

            return Result<IEnumerable<CollaboratorDTO>>.Success(collabs.Select(_mapper.Map<CollaboratorDTO>));

        }
        catch (Exception e)
        {
            return Result<IEnumerable<CollaboratorDTO>>.Failure(Error.InternalServerError(e.Message));
        }
    }


    //UC16: Como gestor de projeto, quero listar quantos dias de férias um colaborador tem marcado durante um projeto
    public async Task<Result<int>> GetHolidayDaysOfCollaboratorInProjectAsync(Guid projectId, Guid collaboratorId)
    {
        try
        {
            var association = await _associationProjectCollaboratorRepository.FindByProjectAndCollaboratorAsync(projectId, collaboratorId) ?? throw new Exception("A associação com os parâmetros fornecidos não existe.");

            int numberOfHolidayDays = 0;

            var collaboratorHolidayPlan = await _holidayPlanRepository.FindHolidayPlanByCollaboratorAsync(collaboratorId);

            if (collaboratorHolidayPlan == null)
                return Result<int>.Success(numberOfHolidayDays);

            numberOfHolidayDays = collaboratorHolidayPlan.GetNumberOfHolidayDaysBetween(
                association.PeriodDate
            );

            return Result<int>.Success(numberOfHolidayDays);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(Error.InternalServerError(ex.Message));
        }
    }


    // UC19 - Given a collaborator and a period to search for, return the holiday periods that contain weekends.
    public async Task<IEnumerable<IHolidayPeriod>> FindAllHolidayPeriodsForCollaboratorBetweenDatesThatIncludeWeekendsAsync(Guid collaboratorId, PeriodDate searchPeriod)
    {
        if (!searchPeriod.ContainsWeekend())
            return Enumerable.Empty<IHolidayPeriod>();

        IEnumerable<IHolidayPeriod> holidayPeriodsBetweenDates =
            await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorBetweenDatesAsync(collaboratorId, searchPeriod);

        IEnumerable<IHolidayPeriod> hp = holidayPeriodsBetweenDates
            .Where(hp => hp.ContainsWeekend());

        return hp;
    }

    // UC20 - Given 2 collaborators and a period to search for, return the overlapping holiday periods they have.
    public async Task<IEnumerable<IHolidayPeriod>> FindAllOverlappingHolidayPeriodsBetweenTwoCollaboratorsBetweenDatesAsync(Guid collaboratorId1, Guid collaboratorId2, PeriodDate searchPeriod)
    {
        IEnumerable<IHolidayPeriod> holidayPeriodListColab1 =
            await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorBetweenDatesAsync(collaboratorId1, searchPeriod);

        IEnumerable<IHolidayPeriod> holidayPeriodListColab2 =
            await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorBetweenDatesAsync(collaboratorId2, searchPeriod);

        IEnumerable<IHolidayPeriod> hp = holidayPeriodListColab1
            .SelectMany(period1 => holidayPeriodListColab2
                    .Where(period2 => period1.Intersects(period2))
                    .SelectMany(period2 => new List<IHolidayPeriod> { period1, period2 }))
                    .Distinct();

        return hp;
    }
    private async Task<IEnumerable<IHolidayPeriod>> GetIntersectingHolidayPeriodsForProjectCollaboratorsAsync(Guid projectId, PeriodDate period)
    {
        var associations = await _associationProjectCollaboratorRepository.FindAllByProjectAndIntersectingPeriodAsync(projectId, period);
        var collaboratorIds = associations.Select(a => a.CollaboratorId).Distinct().ToList();

        return await _holidayPlanRepository.FindAllHolidayPeriodsForAllCollaboratorsIntersectingPeriodAsync(collaboratorIds, period);
    }

    public async Task<Result<IEnumerable<HolidayPeriodDTO>>> FindAllHolidayPeriodsForAllProjectCollaboratorsBetweenDatesAsync(Guid projectId, PeriodDate period)
    {
        try
        {
            var associations = await _associationProjectCollaboratorRepository.FindAllByProjectAsync(projectId);
            var collabs = associations.Select(a => a.CollaboratorId).ToList();

            var holidayPeriods = await _holidayPlanRepository.FindAllHolidayPeriodsForAllCollaboratorsBetweenDatesAsync(collabs, period);

            var result = holidayPeriods.Select(_mapper.Map<HolidayPeriodDTO>);

            return Result<IEnumerable<HolidayPeriodDTO>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<HolidayPeriodDTO>>.Failure(Error.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<int>> GetHolidayDaysForProjectCollaboratorBetweenDates(Guid projectId, Guid collaboratorId, PeriodDate periodDate)
    {
        try
        {
            var associations = await _associationProjectCollaboratorRepository.FindAllByProjectAndCollaboratorAndBetweenPeriodAsync(projectId, collaboratorId, periodDate);

            int totalHolidayDays = 0;
            var holidayPeriods = await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorIntersectingPeriodDate(collaboratorId, periodDate);

            foreach (var holidayColabPeriod in holidayPeriods)
            {
                totalHolidayDays += holidayColabPeriod.GetNumberOfCommonUtilDaysBetweenPeriods(periodDate);
            }

            return Result<int>.Success(totalHolidayDays);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(Error.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<int>> GetHolidayDaysForProjectAllCollaboratorBetweenDates(Guid projectId, PeriodDate searchPeriod)
    {
        try
        {
            var holidayPeriods = await GetIntersectingHolidayPeriodsForProjectCollaboratorsAsync(projectId, searchPeriod);

            var result = holidayPeriods.Sum(period =>
                period.GetNumberOfCommonUtilDaysBetweenPeriods(searchPeriod));

            return Result<int>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(Error.InternalServerError(ex.Message));
        }
    }

    public async Task<HolidayPeriod?> FindHolidayPeriodForCollaboratorThatContainsDay(Guid collabId, DateOnly dateOnly)
    {
        var holidayPeriods = await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorAsync(collabId);

        return holidayPeriods.Where(h => h.ContainsDate(dateOnly)).FirstOrDefault();
    }

    public async Task<IEnumerable<HolidayPeriod>> FindAllHolidayPeriodsForCollaboratorLongerThan(Guid collabId, int amount)
    {
        var holidayPeriods = await _holidayPlanRepository.FindHolidayPeriodsByCollaboratorAsync(collabId);

        return holidayPeriods.Where(h => h.GetDuration() > amount);
    }


}
