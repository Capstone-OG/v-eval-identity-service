using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Common.Validators;
using MediatR;

namespace Application.Features.Auth.Queries.ValidateEmail;

public sealed record ValidateEmailResponseDto(
    bool IsValid,
    bool IsAvailable,
    bool HasMxRecord,
    string Email,
    string Domain,
    string Message,
    string? SuggestedEmail = null
);

public sealed record ValidateEmailQuery(string Email) : IRequest<Result<ValidateEmailResponseDto>>;

public sealed class ValidateEmailHandler : IRequestHandler<ValidateEmailQuery, Result<ValidateEmailResponseDto>>
{
    private readonly IUserRepository _userRepository;

    public ValidateEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<ValidateEmailResponseDto>> Handle(
        ValidateEmailQuery request,
        CancellationToken cancellationToken)
    {
        var check = await EmailValidationHelper.ValidateEmailComprehensiveAsync(request.Email, cancellationToken);
        if (!check.IsValid)
        {
            return Result<ValidateEmailResponseDto>.Success(new ValidateEmailResponseDto(
                IsValid: false,
                IsAvailable: false,
                HasMxRecord: check.HasMxRecord,
                Email: check.NormalizedEmail,
                Domain: check.Domain,
                Message: check.Message,
                SuggestedEmail: check.SuggestedEmail
            ));
        }

        var existingUser = await _userRepository.GetByEmailAsync(check.NormalizedEmail, cancellationToken);
        if (existingUser != null && existingUser.IsActive)
        {
            return Result<ValidateEmailResponseDto>.Success(new ValidateEmailResponseDto(
                IsValid: false,
                IsAvailable: false,
                HasMxRecord: check.HasMxRecord,
                Email: check.NormalizedEmail,
                Domain: check.Domain,
                Message: "Email này đã được đăng ký và kích hoạt bởi một tài khoản khác."
            ));
        }

        return Result<ValidateEmailResponseDto>.Success(new ValidateEmailResponseDto(
            IsValid: true,
            IsAvailable: true,
            HasMxRecord: check.HasMxRecord,
            Email: check.NormalizedEmail,
            Domain: check.Domain,
            Message: "Email hợp lệ (Đã xác minh máy chủ nhận thư MX & sẵn sàng đăng ký)."
        ));
    }
}

