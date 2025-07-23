using Domain.Models;

namespace Application.IServices;

public interface ICollaboratorService
{
    Task AddConsumedCollaborator(Guid id, PeriodDateTime periodDateTime);
}