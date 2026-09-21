using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand : IRequest<Result<string>>
{
    public string Email { get; init; } = string.Empty;
    public string OtpCode { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Định dạng email không hợp lệ.");

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("Mã OTP không được để trống.")
            .Length(6).WithMessage("Mã OTP phải có đúng 6 chữ số.")
            .Matches(@"^[0-9]{6}$").WithMessage("Mã OTP chỉ bao gồm chữ số.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa.")
            .Matches(@"[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường.")
            .Matches(@"[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.")
            .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp.");
    }
}

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    private readonly IOtpRepository _otpRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordHandler(
        IOtpRepository otpRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _otpRepository = otpRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 1. Kiểm tra mã OTP loại RESET_PASSWORD hợp lệ
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

        // 2. Tìm người dùng
        var user = await _userRepository.GetByEmailAsync(emailNormalized, cancellationToken);
        if (user == null)
        {
            return Result<string>.Failure(
                Error.NotFound("Auth.UserNotFound", "Không tìm thấy người dùng với email này."));
        }

        // 3. Đánh dấu OTP đã sử dụng
        otp.IsUsed = true;
        _otpRepository.Update(otp);

        // 4. Băm và cập nhật mật khẩu mới
        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        _userRepository.Update(user);

        // 5. Thu hồi tất cả Refresh Token cũ của User để đảm bảo bảo mật (bắt buộc đăng nhập lại trên mọi thiết bị)
        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.UserId, cancellationToken);

        // 6. Lưu tất cả thay đổi
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success("Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập bằng mật khẩu mới.");
    }
}
