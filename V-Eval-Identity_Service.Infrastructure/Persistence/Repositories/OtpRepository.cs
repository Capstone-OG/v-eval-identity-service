using Application.Common.Interfaces.Repositories;
using Domain.Entities.Iam;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OtpRepository : GenericRepository<OtpVerification>, IOtpRepository
{
    public OtpRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<OtpVerification?> GetLatestValidOtpAsync(
        string email,
        string otpCode,
        string type,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.Email == email &&
                        o.OtpCode == otpCode &&
                        o.Type == type &&
                        !o.IsUsed &&
                        o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
