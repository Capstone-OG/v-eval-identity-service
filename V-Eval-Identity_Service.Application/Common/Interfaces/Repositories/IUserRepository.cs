using Domain.Entities.Iam;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
}
