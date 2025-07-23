using Application.IServices;
using Domain.Messages;
using MassTransit;

public class AssociationProjectCollaboratorCreatedConsumer : IConsumer<AssociationProjectCollaboratorCreatedMessage>
{
    private readonly IAssociationProjectCollaboratorService _associationService;

    public AssociationProjectCollaboratorCreatedConsumer(IAssociationProjectCollaboratorService associationService)
    {
        _associationService = associationService;
    }

    public async Task Consume(ConsumeContext<AssociationProjectCollaboratorCreatedMessage> context)
    {
        var msg = context.Message;
        await _associationService.AddConsumedAssociationProjCollab(msg.Id, msg.ProjectId, msg.CollaboratorId, msg.PeriodDate);
    }
}