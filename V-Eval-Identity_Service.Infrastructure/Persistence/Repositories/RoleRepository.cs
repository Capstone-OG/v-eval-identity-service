using Application.Common.Interfaces.Repositories;
using Domain.Entities.Iam;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }
}
