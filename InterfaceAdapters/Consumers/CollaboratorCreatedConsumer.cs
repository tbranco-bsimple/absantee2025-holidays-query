using Application.IServices;
using Domain.Messages;
using MassTransit;

public class CollaboratorCreatedConsumer : IConsumer<CollaboratorCreatedMessage>
{
    private readonly ICollaboratorService _collaboratorService;

    public CollaboratorCreatedConsumer(ICollaboratorService collaboratorService)
    {
        _collaboratorService = collaboratorService;
    }

    public async Task Consume(ConsumeContext<CollaboratorCreatedMessage> context)
    {
        var msg = context.Message;
        await _collaboratorService.AddConsumedCollaborator(msg.Id, msg.PeriodDateTime);
    }
}