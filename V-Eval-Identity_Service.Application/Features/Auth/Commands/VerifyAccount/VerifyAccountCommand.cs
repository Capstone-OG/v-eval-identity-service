using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Auth.Commands.VerifyAccount;

public record VerifyAccountCommand : IRequest<Result<string>>
{
    public string Email { get; init; } = string.Empty;
    public string OtpCode { get; init; } = string.Empty;
}

public class VerifyAccountValidator : AbstractValidator<VerifyAccountCommand>
{
    public VerifyAccountValidator()
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

public class VerifyAccountHandler : IRequestHandler<VerifyAccountCommand, Result<string>>
{
    private readonly IOtpRepository _otpRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyAccountHandler(
        IOtpRepository otpRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _otpRepository = otpRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(
        VerifyAccountCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToLowerInvariant();

        // 1. Kiểm tra mã OTP hợp lệ
        var otp = await _otpRepository.GetLatestValidOtpAsync(
            emailNormalized,
            request.OtpCode.Trim(),
            "REGISTER_VERIFY",
            cancellationToken);

        if (otp == null)
        {
            return Result<string>.Failure(
                Error.Validation("Auth.InvalidOtp", "Mã xác thực OTP không chính xác hoặc đã hết hạn."));
        }

        // 2. Lấy thông tin User
        var user = await _userRepository.GetByEmailAsync(emailNormalized, cancellationToken);
        if (user == null)
        {
            return Result<string>.Failure(
                Error.NotFound("Auth.UserNotFound", "Không tìm thấy người dùng với email này."));
        }

        // 3. Kích hoạt tài khoản và đánh dấu OTP đã dùng
        otp.IsUsed = true;
        _otpRepository.Update(otp);

        user.IsActive = true;
        _userRepository.Update(user);

        // 4. Lưu thay đổi qua Unit of Work
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success("Tài khoản đã được kích hoạt thành công. Bạn có thể đăng nhập ngay bây giờ.");
    }
}
