using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.Logout;

public record LogoutCommand : IRequest<Result<string>>
{
    public string RefreshToken { get; init; } = string.Empty;
}

public class LogoutValidator : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token không được để trống.");
    }
}

public class LogoutHandler : IRequestHandler<LogoutCommand, Result<string>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken.Trim(), cancellationToken);
        if (existingToken != null && !existingToken.IsRevoked)
        {
            existingToken.IsRevoked = true;
            _refreshTokenRepository.Update(existingToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<string>.Success("Đăng xuất thành công.");
    }
}
