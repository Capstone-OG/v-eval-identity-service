using Domain.Entities.Iam;

namespace Application.Common.Interfaces.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
}
