using Application.DTO;
using Domain.Interfaces;
using Domain.Models;

namespace Application.IServices;

public interface IHolidayPlanService
{
    Task AddConsumedHolidayPeriod(Guid holidayPlanId, Guid id, PeriodDate periodDate);
    Task AddConsumedHolidayPlan(Guid id, Guid collabId, List<HolidayPeriod> holidayPeriods);
    Task<Result<IEnumerable<CollaboratorDTO>>> FindAllCollaboratorsWithHolidayPeriodsBetweenDates(PeriodDate periodDate);
    Task<Result<IEnumerable<HolidayPeriodDTO>>> FindAllHolidayPeriodsForAllProjectCollaboratorsBetweenDatesAsync(Guid projectId, PeriodDate period);
    Task<IEnumerable<IHolidayPeriod>> FindAllHolidayPeriodsForCollaboratorBetweenDatesThatIncludeWeekendsAsync(Guid collaboratorId, PeriodDate searchPeriod);
    Task<IEnumerable<HolidayPeriod>> FindAllHolidayPeriodsForCollaboratorLongerThan(Guid collabId, int amount);
    Task<IEnumerable<IHolidayPeriod>> FindAllOverlappingHolidayPeriodsBetweenTwoCollaboratorsBetweenDatesAsync(Guid collaboratorId1, Guid collaboratorId2, PeriodDate searchPeriod);
    Task<Result<IEnumerable<CollaboratorDTO>>> FindAllWithHolidayPeriodsLongerThan(int days);
    Task<IEnumerable<HolidayPeriodDTO>> FindHolidayPeriodForCollaborator(Guid collaboratorId);
    Task<HolidayPeriod?> FindHolidayPeriodForCollaboratorThatContainsDay(Guid collabId, DateOnly dateOnly);
    Task<IEnumerable<HolidayPeriodDTO>> FindHolidayPeriodsByCollaboratorBetweenDatesAsync(Guid collaboratorId, PeriodDate periodDate);
    Task<Result<int>> GetHolidayDaysForProjectAllCollaboratorBetweenDates(Guid projectId, PeriodDate searchPeriod);
    Task<Result<int>> GetHolidayDaysForProjectCollaboratorBetweenDates(Guid projectId, Guid collaboratorId, PeriodDate periodDate);
    Task<Result<int>> GetHolidayDaysOfCollaboratorInProjectAsync(Guid projectId, Guid collaboratorId);
    Task UpdateConsumedHolidayPeriod(Guid id, PeriodDate periodDate);
}