using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.IRepository;
using Domain.Models;
using Domain.Visitor;

namespace Domain.Factory;

public class HolidayPeriodFactory : IHolidayPeriodFactory
{
    private readonly IHolidayPlanRepository _holidayPlanRepository;
    private readonly ICollaboratorRepository _collaboratorRepository;

    public HolidayPeriodFactory(IHolidayPlanRepository holidayPlanRepository, ICollaboratorRepository collaboratorRepository)
    {
        _holidayPlanRepository = holidayPlanRepository;
        _collaboratorRepository = collaboratorRepository;
    }

    public async Task<HolidayPeriod> Create(Guid holidayPlanId, DateOnly initDate, DateOnly finalDate)
    {
        PeriodDate periodDate = new PeriodDate(initDate, finalDate);
        HolidayPeriod holidayPeriod = new HolidayPeriod(periodDate);

        if (!await _holidayPlanRepository.CanInsertHolidayPeriod(holidayPlanId, holidayPeriod))
            throw new ArgumentException("Holiday Period already exists for this Holiday Plan.");

        var holidayPlan = await _holidayPlanRepository.GetByIdAsync(holidayPlanId);
        if (holidayPlan == null)
            throw new ArgumentException("Holiday Plan doesn't exist.");

        Guid collaboratorId = holidayPlan.CollaboratorId;
        var collaborator = await _collaboratorRepository.GetByIdAsync(collaboratorId);
        if (collaborator == null)
            throw new ArgumentException("Collaborator doesn't exist.");

        return holidayPeriod;
    }

    public HolidayPeriod CreateWithoutHolidayPlan(ICollaborator collaborator, DateOnly initDate, DateOnly finalDate)
    {
        PeriodDate periodDate = new PeriodDate(initDate, finalDate);

        return new HolidayPeriod(periodDate);
    }

    public HolidayPeriod Create(IHolidayPeriodVisitor visitor)
    {
        return new HolidayPeriod(visitor.Id, visitor.PeriodDate);
    }
}