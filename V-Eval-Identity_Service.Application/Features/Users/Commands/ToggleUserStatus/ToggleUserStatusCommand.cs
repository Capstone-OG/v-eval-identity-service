using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using MediatR;

namespace Application.Features.Users.Commands.ToggleUserStatus;

public record ToggleUserStatusCommand(Guid UserId) : IRequest<Result<bool>>;

public class ToggleUserStatusHandler : IRequestHandler<ToggleUserStatusCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleUserStatusHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        ToggleUserStatusCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<bool>.Failure(Error.NotFound("User.NotFound", "Không tìm thấy người dùng."));
        }

        user.IsActive = !user.IsActive;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(user.IsActive);
    }
}
