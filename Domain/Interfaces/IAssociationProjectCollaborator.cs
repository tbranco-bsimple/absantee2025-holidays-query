using Domain.Models;

namespace Domain.Interfaces;

public interface IAssociationProjectCollaborator
{
    public Guid Id { get; }
    public Guid ProjectId { get; }
    public Guid CollaboratorId { get; }
    public PeriodDate PeriodDate { get; }
}
