using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Features.Auth.Commands.Register;
using Domain.Constants;
using Domain.Entities.Iam;
using Domain.Entities.Profiles;
using FluentAssertions;
using Moq;
using Xunit;

namespace V_Eval_Identity_Service.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IRoleRepository> _roleRepoMock = new();
    private readonly Mock<IStudentRepository> _studentRepoMock = new();
    private readonly Mock<IOtpRepository> _otpRepoMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IOtpService> _otpServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly RegisterUserHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserHandler(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _studentRepoMock.Object,
            _otpRepoMock.Object,
            _passwordHasherMock.Object,
            _otpServiceMock.Object,
            _emailServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidStudentRegistration_ShouldCallEmailService_AndReturnSuccess()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Email = "student@test.com",
            Password = "Password123!",
            FullName = "Nguyễn Văn Test",
            Phone = "0987654321",
            RoleName = SystemRoles.Student,
            CampusId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")
        };

        _userRepoMock.Setup(x => x.IsEmailUniqueAsync("student@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roleRepoMock.Setup(x => x.GetByNameAsync(SystemRoles.Student, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { RoleId = Guid.NewGuid(), RoleName = SystemRoles.Student });

        _passwordHasherMock.Setup(x => x.Hash(It.IsAny<string>()))
            .Returns("hashed_secret");

        _otpServiceMock.Setup(x => x.GenerateOtpCode())
            .Returns("654321");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("student@test.com");
        result.Value.OtpCode.Should().Be("654321");

        // Verify email service was invoked with correct parameters
        _emailServiceMock.Verify(x => x.SendRegistrationOtpEmailAsync(
            "student@test.com",
            "Nguyễn Văn Test",
            "654321",
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldReturnConflict_WithoutCallingEmailService()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Email = "existing@test.com",
            Password = "Password123!",
            FullName = "Existing User",
            Phone = "0987654321",
            RoleName = SystemRoles.Student
        };

        _userRepoMock.Setup(x => x.IsEmailUniqueAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Auth.EmailAlreadyExists");

        // Email service must NOT be called
        _emailServiceMock.Verify(x => x.SendRegistrationOtpEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
