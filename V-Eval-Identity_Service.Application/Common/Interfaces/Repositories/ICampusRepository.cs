using Domain.Entities.Iam;

namespace Application.Common.Interfaces.Repositories;

public interface ICampusRepository : IGenericRepository<Campus>
{
    Task<IReadOnlyList<Campus>> GetActiveCampusesAsync(CancellationToken cancellationToken = default);
    Task<Campus?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
