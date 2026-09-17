using Domain.Constants;
using Domain.Entities.Iam;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeds;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. Đảm bảo 2 Schema iam và profile tồn tại trong PostgreSQL
        await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS iam;");
        await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS profile;");

        // 2. Seed 6 Roles cơ bản nếu chưa tồn tại
        var existingRoles = await context.Roles.Select(r => r.RoleName).ToListAsync();

        var rolesToSeed = SystemRoles.All
            .Where(roleName => !existingRoles.Contains(roleName))
            .Select(roleName => new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = roleName
            })
            .ToList();

        if (rolesToSeed.Count != 0)
        {
            await context.Roles.AddRangeAsync(rolesToSeed);
            await context.SaveChangesAsync();
        }
    }
}
