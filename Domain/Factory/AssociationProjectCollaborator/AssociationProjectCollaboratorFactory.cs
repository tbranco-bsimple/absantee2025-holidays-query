using Domain.Interfaces;
using Domain.IRepository;
using Domain.Models;
using Domain.Visitor;

namespace Domain.Factory;

public class AssociationProjectCollaboratorFactory : IAssociationProjectCollaboratorFactory
{
    public AssociationProjectCollaboratorFactory()
    {
    }

    public AssociationProjectCollaborator Create(IAssociationProjectCollaboratorVisitor associationProjectCollaboratorVisitor)
    {
        return new AssociationProjectCollaborator(
                    associationProjectCollaboratorVisitor.Id,
                    associationProjectCollaboratorVisitor.ProjectId,
                    associationProjectCollaboratorVisitor.CollaboratorId,
                    associationProjectCollaboratorVisitor.PeriodDate);
    }
}