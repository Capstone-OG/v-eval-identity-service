using Domain.Entities.Iam;

namespace Application.Common.Interfaces;

public interface IJwtProvider
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
