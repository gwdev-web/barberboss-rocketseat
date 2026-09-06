using BarberBoss.Application.UseCases.Users.Profile;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Tests.Users.Profile;

public class GetUserProfileUseCaseTests
{
    [Fact]
    public async Task Success_Own_Profile()
    {
        var user = UserBuilder.Build();

        var useCase = new GetUserProfileUseCase(
            new UsersReadOnlyRepositoryBuilder().GetById(user).Build(),
            LoggedUserBuilder.Build(user),
            MapperBuilder.Build());

        var result = await useCase.Execute();

        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Error_Another_User_Profile()
    {
        var loggedUser = UserBuilder.Build();
        var otherUser = UserBuilder.Build();

        var useCase = new GetUserProfileUseCase(
            new UsersReadOnlyRepositoryBuilder().GetById(otherUser).Build(),
            LoggedUserBuilder.Build(loggedUser),
            MapperBuilder.Build());

        var act = async () => await useCase.Execute(otherUser.Id);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
