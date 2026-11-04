using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyResetOtp;

public record VerifyResetOtpCommand : IRequest<Result<string>>
{
    public string Email { get; init; } = string.Empty;
    public string OtpCode { get; init; } = string.Empty;
}

public class VerifyResetOtpValidator : AbstractValidator<VerifyResetOtpCommand>
{
    public VerifyResetOtpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ.");

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("Mã OTP không được để trống.")
            .Length(6).WithMessage("Mã OTP phải có đúng 6 chữ số.")
            .Matches(@"^[0-9]{6}$").WithMessage("Mã OTP chỉ bao gồm chữ số.");
    }
}

public class VerifyResetOtpHandler : IRequestHandler<VerifyResetOtpCommand, Result<string>>
{
    private readonly IOtpRepository _otpRepository;

    public VerifyResetOtpHandler(IOtpRepository otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public async Task<Result<string>> Handle(
        VerifyResetOtpCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 1. Kiểm tra mã OTP loại RESET_PASSWORD hợp lệ trong CSDL
        var otp = await _otpRepository.GetLatestValidOtpAsync(
            emailNormalized,
            request.OtpCode.Trim(),
            "RESET_PASSWORD",
            cancellationToken);

        if (otp == null)
        {
            return Result<string>.Failure(
                Error.Validation("Auth.InvalidOtp", "Mã xác thực OTP không chính xác hoặc đã hết hạn."));
        }

        return Result<string>.Success("Mã xác thực OTP hợp lệ.");
    }
}

