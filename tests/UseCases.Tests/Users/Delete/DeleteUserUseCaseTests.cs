using BarberBoss.Application.UseCases.Users.Delete;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Tests.Users.Delete;

public class DeleteUserUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();

        var useCase = new DeleteUserUseCase(
            new UsersWriteOnlyRepositoryBuilder().Delete(user.Id, result: true).Build(),
            LoggedUserBuilder.Build(user),
            UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(user.Id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_Deleting_Another_User()
    {
        var loggedUser = UserBuilder.Build();
        var otherUserId = Guid.NewGuid();

        var useCase = new DeleteUserUseCase(
            new UsersWriteOnlyRepositoryBuilder().Delete(otherUserId, result: true).Build(),
            LoggedUserBuilder.Build(loggedUser),
            UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(otherUserId);

        var exception = await act.Should().ThrowAsync<ForbiddenException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.NO_PERMISSION));
    }
}
