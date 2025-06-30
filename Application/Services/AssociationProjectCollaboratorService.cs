using Domain.Factory;
using Domain.Models;
using Domain.IRepository;
using AutoMapper;
using Application.DTO;
using Infrastructure.DataModel;

namespace Application.Services;

public class AssociationProjectCollaboratorService
{
    private IAssociationProjectCollaboratorRepository _assocRepository;
    private IAssociationProjectCollaboratorFactory _associationProjectCollaboratorFactory;

    public AssociationProjectCollaboratorService(IAssociationProjectCollaboratorRepository assocRepository, IAssociationProjectCollaboratorFactory associationProjectCollaboratorFactory)
    {
        _assocRepository = assocRepository;
        _associationProjectCollaboratorFactory = associationProjectCollaboratorFactory;
    }

    public async Task SubmitAssociationProjCollabAsync(Guid id, Guid projectId, Guid collaboratorId, PeriodDate periodDate)
    {
        var visitor = new AssociationProjectCollaboratorDataModel()
        {
            Id = id,
            ProjectId = projectId,
            CollaboratorId = collaboratorId,
            PeriodDate = periodDate
        };

        var association = _associationProjectCollaboratorFactory.Create(visitor);

        await _assocRepository.AddAsync(association);
    }
}
