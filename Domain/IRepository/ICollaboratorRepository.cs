using Domain.Interfaces;
using Domain.Models;
using Domain.Visitor;

namespace Domain.IRepository;

public interface ICollaboratorRepository : IGenericRepositoryEF<ICollaborator, Collaborator, ICollaboratorVisitor>
{
    Task<IEnumerable<ICollaborator>> GetByIdsAsync(IEnumerable<Guid> ids);
}
