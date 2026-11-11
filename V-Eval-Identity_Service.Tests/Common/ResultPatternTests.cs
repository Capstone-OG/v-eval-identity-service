using System;
using Application.Common.Models;
using Xunit;

namespace V_Eval_Identity_Service.Tests.Common;

public class ResultPatternTests
{
    [Fact]
    public void Success_ShouldSetIsSuccessTrue_AndNoError()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_ShouldSetIsSuccessFalse_AndContainError()
    {
        // Arrange
        var customError = Error.Validation("Auth.InvalidCredentials", "Thông tin đăng nhập không hợp lệ.");

        // Act
        var result = Result.Failure(customError);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(customError, result.Error);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void FailureWithValue_WhenAccessingValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = Error.NotFound("User.NotFound", "Không tìm thấy người dùng.");
        var failureResult = Result<string>.Failure(error);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => failureResult.Value);
    }
}
