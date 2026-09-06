using BarberBoss.Application.UseCases.Users;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Users;

public class RegisterUserValidatorTests
{
    [Fact]
    public void Success()
    {
        var result = new RegisterUserValidator().Validate(RequestRegisterUserJsonBuilder.Build());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Error_Name_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        var result = new RegisterUserValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.NAME_REQUIRED));
    }

    [Theory]
    [InlineData("sem-arroba")]
    [InlineData("faltando@")]
    public void Error_Email_Invalid(string email)
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = email;

        var result = new RegisterUserValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.EMAIL_INVALID));
    }

    [Fact]
    public void Error_Email_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = string.Empty;

        var result = new RegisterUserValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.EMAIL_REQUIRED));
    }

    [Fact]
    public void Error_Password_Too_Short()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = "123";

        var result = new RegisterUserValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_MIN_LENGTH));
    }
}

public class LoginValidatorTests
{
    [Fact]
    public void Success()
    {
        var result = new LoginValidator().Validate(RequestLoginJsonBuilder.Build("rafael@barberboss.com"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Error_Password_Empty()
    {
        var request = RequestLoginJsonBuilder.Build("rafael@barberboss.com", string.Empty);

        var result = new LoginValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_REQUIRED));
    }
}
