using BarberBoss.Application.UseCases.Login.DoLogin;
using BarberBoss.Domain.Entities;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using FluentAssertions;

namespace UseCases.Tests.Login;

public class DoLoginUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build(user.Email, UserBuilder.DEFAULT_PASSWORD);

        var result = await CreateUseCase(user).Execute(request);

        result.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        var request = RequestLoginJsonBuilder.Build("nao-existe@barberboss.com");

        var act = async () => await CreateUseCase(null).Execute(request);

        var exception = await act.Should().ThrowAsync<InvalidLoginException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID));
    }

    [Fact]
    public async Task Error_Wrong_Password()
    {
        var user = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build(user.Email, "senha-errada");

        var act = async () => await CreateUseCase(user).Execute(request);

        await act.Should().ThrowAsync<InvalidLoginException>();
    }

    private static DoLoginUseCase CreateUseCase(User? user)
        => new(
            new UsersReadOnlyRepositoryBuilder().GetByEmail(user).Build(),
            PasswordEncripterBuilder.Build(),
            AccessTokenGeneratorBuilder.Build());
}
