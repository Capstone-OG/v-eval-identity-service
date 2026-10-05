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

        // 3. Seed tài khoản Admin: admin / 1234 (admin@veval.edu.vn)
        var adminEmail = "admin@veval.edu.vn";
        var existingAdmin = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == adminEmail || u.Email == "admin");

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == SystemRoles.Administrator);

        if (adminRole != null)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("1234", workFactor: 11);

            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = adminEmail,
                    FullName = "admin",
                    Phone = "0900000000",
                    PasswordHash = passwordHash,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                adminUser.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.RoleId
                });

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
            else
            {
                existingAdmin.PasswordHash = passwordHash;
                existingAdmin.FullName = "admin";
                existingAdmin.IsActive = true;
                if (!existingAdmin.UserRoles.Any(ur => ur.RoleId == adminRole.RoleId))
                {
                    existingAdmin.UserRoles.Add(new UserRole
                    {
                        UserId = existingAdmin.UserId,
                        RoleId = adminRole.RoleId
                    });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
