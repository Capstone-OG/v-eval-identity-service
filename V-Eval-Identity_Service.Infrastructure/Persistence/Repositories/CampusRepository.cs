using Application.Common.Interfaces.Repositories;
using Domain.Entities.Iam;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CampusRepository : GenericRepository<Campus>, ICampusRepository
{
    public CampusRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Campus>> GetActiveCampusesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Campus?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }
}
