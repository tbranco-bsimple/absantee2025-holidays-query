using Domain.Models;
using Domain.Visitor;

namespace Domain.Factory;

public class CollaboratorFactory : ICollaboratorFactory
{

    public CollaboratorFactory() { }

    public Collaborator Create(ICollaboratorVisitor visitor)
    {
        return new Collaborator(visitor.Id, visitor.PeriodDateTime);
    }
}