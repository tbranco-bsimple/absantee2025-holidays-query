using Domain.Models;

namespace Domain.Messages;

public record CollaboratorCreatedMessage(Guid Id, PeriodDateTime PeriodDateTime);