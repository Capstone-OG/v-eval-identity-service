using Domain.Entities.Iam;

namespace Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}
