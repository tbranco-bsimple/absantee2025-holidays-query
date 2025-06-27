using Domain.Models;

namespace Domain.Messages;

public record AssociationProjectCollaboratorMessage(Guid Id, Guid CollaboratorId, Guid ProjectId, PeriodDate PeriodDate);