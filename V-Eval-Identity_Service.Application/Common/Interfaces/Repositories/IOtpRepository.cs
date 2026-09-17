using Domain.Entities.Iam;

namespace Application.Common.Interfaces.Repositories;

public interface IOtpRepository : IGenericRepository<OtpVerification>
{
    Task<OtpVerification?> GetLatestValidOtpAsync(string email, string otpCode, string type, CancellationToken cancellationToken = default);
}
