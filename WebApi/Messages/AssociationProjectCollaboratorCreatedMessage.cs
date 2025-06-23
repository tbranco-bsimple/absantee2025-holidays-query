using Domain.Models;

namespace WebApi.Messages;

public record AssociationProjectCollaboratorMessage(Guid Id, Guid CollaboratorId, Guid ProjectId, PeriodDate PeriodDate);