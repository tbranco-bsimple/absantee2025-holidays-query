using Domain.Interfaces;

namespace Domain.Models;

public class AssociationProjectCollaborator : IAssociationProjectCollaborator
{
    public Guid Id { get; }
    public Guid ProjectId { get; }
    public Guid CollaboratorId { get; }
    public PeriodDate PeriodDate { get; }

    public AssociationProjectCollaborator(Guid projectId, Guid collaboratorId, PeriodDate periodDate)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        CollaboratorId = collaboratorId;
        PeriodDate = periodDate;
    }
    public AssociationProjectCollaborator(Guid id, Guid projectId, Guid collaboratorId, PeriodDate periodDate)
    {
        Id = id;
        ProjectId = projectId;
        CollaboratorId = collaboratorId;
        PeriodDate = periodDate;
    }

}
