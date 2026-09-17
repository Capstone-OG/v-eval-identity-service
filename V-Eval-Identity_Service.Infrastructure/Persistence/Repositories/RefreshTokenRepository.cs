using Application.Common.Interfaces.Repositories;
using Domain.Entities.Iam;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
    }
}
