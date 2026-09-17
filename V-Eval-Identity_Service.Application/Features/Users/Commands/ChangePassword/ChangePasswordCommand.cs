using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Users.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<Result<string>>
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự.")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ hoa.")
            .Matches(@"[a-z]").WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ thường.")
            .Matches(@"[0-9]").WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ số.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Xác nhận mật khẩu mới không được để trống.")
            .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp với mật khẩu mới.");
    }
}

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<string>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Result<string>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa được xác thực."));
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);
        if (user == null)
        {
            return Result<string>.Failure(
                Error.NotFound("User.NotFound", "Không tìm thấy thông tin người dùng."));
        }

        // 1. Kiểm tra mật khẩu hiện tại
        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return Result<string>.Failure(
                Error.Validation("User.InvalidCurrentPassword", "Mật khẩu hiện tại không chính xác."));
        }

        // 2. Kiểm tra mật khẩu mới không được trùng mật khẩu cũ
        if (_passwordHasher.Verify(request.NewPassword, user.PasswordHash))
        {
            return Result<string>.Failure(
                Error.Validation("User.SamePassword", "Mật khẩu mới không được trùng với mật khẩu hiện tại."));
        }

        // 3. Hash mật khẩu mới và cập nhật
        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success("Đổi mật khẩu thành công.");
    }
}
