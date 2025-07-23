using Domain.IRepository;
using Domain.Factory;
using Domain.Models;
using Infrastructure.DataModel;
using Application.IServices;

namespace Application.Services;

public class CollaboratorService : ICollaboratorService
{
    private readonly ICollaboratorRepository _collaboratorRepository;
    private readonly ICollaboratorFactory _collaboratorFactory;

    public CollaboratorService(ICollaboratorRepository collaboratorRepository, ICollaboratorFactory collaboratorFactory)
    {
        _collaboratorRepository = collaboratorRepository;
        _collaboratorFactory = collaboratorFactory;
    }

    public async Task AddConsumedCollaborator(Guid id, PeriodDateTime periodDateTime)
    {
        var visitor = new CollaboratorDataModel()
        {
            Id = id,
            PeriodDateTime = periodDateTime
        };

        var collaborator = _collaboratorFactory.Create(visitor);

        await _collaboratorRepository.AddAsync(collaborator);
    }

}
