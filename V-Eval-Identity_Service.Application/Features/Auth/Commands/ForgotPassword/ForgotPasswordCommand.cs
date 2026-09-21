using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Features.Auth.DTOs;
using Domain.Entities.Iam;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand : IRequest<Result<ForgotPasswordResponseDto>>
{
    public string Email { get; init; } = string.Empty;
}

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ.")
            .MaximumLength(150).WithMessage("Email không được vượt quá 150 ký tự.");
    }
}

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpRepository _otpRepository;
    private readonly IOtpService _otpService;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordHandler(
        IUserRepository userRepository,
        IOtpRepository otpRepository,
        IOtpService otpService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _otpRepository = otpRepository;
        _otpService = otpService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ForgotPasswordResponseDto>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 1. Kiểm tra tài khoản tồn tại
        var user = await _userRepository.GetByEmailAsync(emailNormalized, cancellationToken);
        if (user == null)
        {
            return Result<ForgotPasswordResponseDto>.Failure(
                Error.NotFound("Auth.UserNotFound", "Không tìm thấy tài khoản với email này."));
        }

        // 2. Kiểm tra tài khoản có đang hoạt động không
        if (!user.IsActive)
        {
            return Result<ForgotPasswordResponseDto>.Failure(
                Error.Validation("Auth.AccountNotActive", "Tài khoản này chưa được kích hoạt hoặc đã bị vô hiệu hóa."));
        }

        // 3. Tạo mã OTP loại RESET_PASSWORD (hạn 10 phút)
        var otpCode = _otpService.GenerateOtpCode();
        var otpVerification = new OtpVerification
        {
            Email = emailNormalized,
            OtpCode = otpCode,
            Type = "RESET_PASSWORD",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _otpRepository.AddAsync(otpVerification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Trả kết quả (kèm otpCode để thuận tiện test trên môi trường Dev)
        var responseDto = new ForgotPasswordResponseDto(
            user.Email,
            "Mã xác thực OTP đặt lại mật khẩu đã được gửi đến email của bạn.",
            otpCode
        );

        return Result<ForgotPasswordResponseDto>.Success(responseDto);
    }
}
