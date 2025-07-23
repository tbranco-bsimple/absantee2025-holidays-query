using Domain.Models;

namespace Application.IServices;

public interface IAssociationProjectCollaboratorService
{
    Task AddConsumedAssociationProjCollab(Guid id, Guid projectId, Guid collaboratorId, PeriodDate periodDate);
}