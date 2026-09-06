using BarberBoss.Application.UseCases.Users.Register;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using FluentAssertions;

namespace UseCases.Tests.Users.Register;

public class RegisterUserUseCaseTests
{
    [Fact]
    public async Task Success_Returns_Token()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(request.Name);
        result.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase(existingEmail: request.Email.ToLowerInvariant());

        var act = async () => await useCase.Execute(request);

        var exception = await act.Should().ThrowAsync<ConflictException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
    }

    [Fact]
    public async Task Error_Short_Password()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = "123";

        var act = async () => await CreateUseCase().Execute(request);

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.PASSWORD_MIN_LENGTH));
    }

    private static RegisterUserUseCase CreateUseCase(string? existingEmail = null)
    {
        var readOnlyBuilder = new UsersReadOnlyRepositoryBuilder();

        if (existingEmail is not null)
            readOnlyBuilder.ExistsUserWithEmail(existingEmail);

        return new RegisterUserUseCase(
            new UsersWriteOnlyRepositoryBuilder().Build(),
            readOnlyBuilder.Build(),
            PasswordEncripterBuilder.Build(),
            AccessTokenGeneratorBuilder.Build(),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());
    }
}
