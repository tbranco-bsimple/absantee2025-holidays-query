using Domain.Models;

namespace Domain.Messages;

public record AssociationProjectCollaboratorCreatedMessage(Guid Id, Guid ProjectId, Guid CollaboratorId, PeriodDate PeriodDate);