using Application.Services;
using Domain.Messages;
using MassTransit;

public class AssociationProjectCollaboratorConsumer : IConsumer<AssociationProjectCollaboratorCreatedMessage>
{
    private readonly AssociationProjectCollaboratorService _associationService;

    public AssociationProjectCollaboratorConsumer(AssociationProjectCollaboratorService associationService)
    {
        _associationService = associationService;
    }

    public async Task Consume(ConsumeContext<AssociationProjectCollaboratorCreatedMessage> context)
    {
        var senderId = context.Headers.Get<string>("SenderId");
        if (senderId == InstanceInfo.InstanceId)
            return;

        var msg = context.Message;
        await _associationService.SubmitAssociationProjCollabAsync(msg.Id, msg.ProjectId, msg.CollaboratorId, msg.PeriodDate);
    }
}