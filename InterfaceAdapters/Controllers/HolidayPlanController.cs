using Application.DTO;
using Application.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace InterfaceAdapters.Controllers;

[Route("api/holidays")]
[ApiController]
public class HolidayPlanController : Controller
{
    private readonly IHolidayPlanService _holidayPlanService;

    public HolidayPlanController(IHolidayPlanService holidayPlanService)
    {
        _holidayPlanService = holidayPlanService;
    }

    [HttpGet("{collaboratorId}")]
    public async Task<ActionResult<IEnumerable<HolidayPeriodDTO>>> GetHolidayPeriodsOfCollaborator(Guid collaboratorId)
    {
        var result = await _holidayPlanService.FindHolidayPeriodForCollaborator(collaboratorId);

        return Ok(result);
    }

    // UC13: Como gestor de RH, quero listar os períodos de férias dum colaborador num período
    [HttpGet("byPeriod/collaborator/{collaboratorId}")]
    public async Task<ActionResult<IEnumerable<HolidayPeriodDTO>>> GetHolidayPeriodsOfCollaboratorByPeriod(Guid collaboratorId, [FromQuery] PeriodDate periodDate)
    {
        var result = await _holidayPlanService.FindHolidayPeriodsByCollaboratorBetweenDatesAsync(collaboratorId, periodDate);

        return Ok(result);
    }

    // UC14: Como gestor de RH, quero listar os colaboradores que têm de férias num período
    [HttpGet("byPeriod/collaborators")]
    public async Task<ActionResult<IEnumerable<CollaboratorDTO>>> GetCollaboratorsByPeriod([FromQuery] DateOnly initDate, [FromQuery] DateOnly finalDate)
    {
        var periodDate = new PeriodDate(initDate, finalDate);
        var result = await _holidayPlanService.FindAllCollaboratorsWithHolidayPeriodsBetweenDates(periodDate);
        return result.ToActionResult();
    }

    // UC15: Como gestor de RH, quero listar os colaboradores que já registaram períodos de férias superiores a x dias
    [HttpGet("longer-than/collaborators")]
    public async Task<ActionResult<IEnumerable<CollaboratorDTO>>> GetWithHolidayPeriodsLongerThan(int days)
    {
        var result = await _holidayPlanService.FindAllWithHolidayPeriodsLongerThan(days);

        return result.ToActionResult();
    }

    // UC16 : Como gestor de projeto, quero listar quantos dias de férias um colaborador tem marcado durante um projeto
    [HttpGet("count/collaborator/{collaboratorId}/project/{projectId}")]
    public async Task<ActionResult<int>> GetHolidayCountByCollaborator(Guid projectId, Guid collaboratorId)
    {
        var count = await _holidayPlanService.GetHolidayDaysOfCollaboratorInProjectAsync(projectId, collaboratorId);
        return count.ToActionResult();
    }

    // UC17: Como colaborador, quero listar o meu período de férias que contém determinado dia (e.g. 18/11/2025)
    [HttpGet("includes-date/collaborator/{collaboratorId}")]
    public async Task<ActionResult<HolidayPeriod?>> GetHolidayPeriodContainingDay(Guid collaboratorId, string date)
    {
        var dateOnly = DateOnly.Parse(date);
        var result = await _holidayPlanService.FindHolidayPeriodForCollaboratorThatContainsDay(collaboratorId, dateOnly);

        if (result != null)
            return Ok(result);

        return NotFound();
    }

    // UC18: Como colaborador, quero listar todos os meu períodos de férias num período, que contém mais do que x dias
    [HttpGet("longer-than/collaborator/{collaboratorId}")]
    public async Task<ActionResult<IEnumerable<HolidayPeriod>>> GetHolidayPeriodLongerThan(Guid collaboratorId, string days)
    {
        var amount = int.Parse(days);
        var result = await _holidayPlanService.FindAllHolidayPeriodsForCollaboratorLongerThan(collaboratorId, amount);

        return Ok(result);
    }

    // UC19: Como colaborador, quero listar todos os meus períodos de férias num período, que “contêm” fins-de-semana
    [HttpGet("includes-weekends/collaborator/{collaboratorId}")]
    public async Task<ActionResult<IEnumerable<HolidayPeriod>>> GetHolidayPeriodsBetweenThatIncludeWeeknds(Guid collaboratorId, PeriodDate periodDate)
    {
        var result = await _holidayPlanService.FindAllHolidayPeriodsForCollaboratorBetweenDatesThatIncludeWeekendsAsync(collaboratorId, periodDate);
        return Ok(result);
    }

    // UC20: Como gestor de RH, quero listar os períodos de férias de dois colaboradores que se sobrepõem, num período
    [HttpGet("overlaps/collaborators/{collaboratorId1}/{collaboratorId2}")]
    public async Task<ActionResult<IEnumerable<HolidayPeriodDTO>>> GetOverlapingPeriodsBetween(Guid collaboratorId1, Guid collaboratorId2, [FromQuery] PeriodDate periodDate)
    {
        var periods = await _holidayPlanService.FindAllOverlappingHolidayPeriodsBetweenTwoCollaboratorsBetweenDatesAsync(collaboratorId1, collaboratorId2, periodDate);

        if (periods == null) return BadRequest();

        return Ok(periods);
    }

    // UC21: Como gestor de projeto, quero listar os períodos de férias dos colaboradores dum projeto, num período
    [HttpGet("byPeriod/project/{projectId}")]
    public async Task<ActionResult<IEnumerable<HolidayPeriodDTO>>> GetHolidaysByProjectAndBetweenPeriod(Guid projectId, [FromQuery] PeriodDate periodDate)
    {
        var holidays = await _holidayPlanService.FindAllHolidayPeriodsForAllProjectCollaboratorsBetweenDatesAsync(projectId, periodDate);
        return holidays.ToActionResult();
    }

    //UC22: Como gestor de projeto, quero listar quantos dias de férias dum colaborador do projeto tem num dado período
    [HttpGet("count/byPeriod/collaborator/{collaboratorId}/project/{projectId}")]
    public async Task<ActionResult<int>> GetHolidayCountByCollaboratorByPeriod(Guid projectId, Guid collaboratorId, [FromQuery] PeriodDate periodDate)
    {
        var count = await _holidayPlanService.GetHolidayDaysForProjectCollaboratorBetweenDates(projectId, collaboratorId, periodDate);
        return count.ToActionResult();
    }

    //UC23: Como gestor de projeto, quero listar a quantidade de dias de férias de todos os colaboradores do projeto num dado período
    [HttpGet("count/byPeriod/project/{projectId}/collaborators")]
    public async Task<ActionResult<int>> GetHolidayCountForAllCollaboratorsByPeriod(Guid projectId, [FromQuery] PeriodDate periodDate)
    {
        var count = await _holidayPlanService.GetHolidayDaysForProjectAllCollaboratorBetweenDates(projectId, periodDate);
        return count.ToActionResult();
    }
}
