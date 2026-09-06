using BarberBoss.Application.UseCases.Users.Update;
using BarberBoss.Communication.Enums;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Tests.Users.Update;

public class UpdateUserUseCaseTests
{
    [Fact]
    public async Task Success_Updating_Own_Profile()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = new UpdateUserUseCase(
            new UsersUpdateOnlyRepositoryBuilder().GetById(user).Build(),
            new UsersReadOnlyRepositoryBuilder().Build(),
            LoggedUserBuilder.Build(user),
            UnitOfWorkBuilder.Build());

        await useCase.Execute(request);

        user.Name.Should().Be(request.Name);
        user.Email.Should().Be(request.Email.ToLowerInvariant());
    }

    [Fact]
    public async Task Error_Updating_Another_User()
    {
        var loggedUser = UserBuilder.Build();
        var otherUser = UserBuilder.Build();

        var useCase = new UpdateUserUseCase(
            new UsersUpdateOnlyRepositoryBuilder().GetById(otherUser).Build(),
            new UsersReadOnlyRepositoryBuilder().Build(),
            LoggedUserBuilder.Build(loggedUser),
            UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(otherUser.Id, RequestUpdateUserJsonBuilder.Build());

        var exception = await act.Should().ThrowAsync<ForbiddenException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.NO_PERMISSION));
    }

    [Fact]
    public async Task Success_Admin_Updating_Another_User()
    {
        var admin = UserBuilder.Build(UserRole.Admin);
        var otherUser = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = new UpdateUserUseCase(
            new UsersUpdateOnlyRepositoryBuilder().GetById(otherUser).Build(),
            new UsersReadOnlyRepositoryBuilder().Build(),
            LoggedUserBuilder.Build(admin),
            UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(otherUser.Id, request);

        await act.Should().NotThrowAsync();
        otherUser.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = new UpdateUserUseCase(
            new UsersUpdateOnlyRepositoryBuilder().GetById(user).Build(),
            new UsersReadOnlyRepositoryBuilder()
                .ExistsUserWithEmail(request.Email.ToLowerInvariant())
                .Build(),
            LoggedUserBuilder.Build(user),
            UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(user.Id, request);

        var exception = await act.Should().ThrowAsync<ConflictException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
    }
}
