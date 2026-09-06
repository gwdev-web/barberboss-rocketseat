using BarberBoss.Application.UseCases.Billings.Update;
using BarberBoss.Domain.Entities;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Tests.Billings.Update;

public class UpdateBillingUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var billing = BillingBuilder.Build(userId: user.Id);
        var request = RequestBillingJsonBuilder.Build();

        var useCase = CreateUseCase(user, billing);

        var act = async () => await useCase.Execute(billing.Id, request);

        await act.Should().NotThrowAsync();

        billing.ClientName.Should().Be(request.ClientName);
        billing.Amount.Should().Be(request.Amount);
        billing.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var user = UserBuilder.Build();
        var useCase = CreateUseCase(user, null);

        var act = async () => await useCase.Execute(Guid.NewGuid(), RequestBillingJsonBuilder.Build());

        var exception = await act.Should().ThrowAsync<NotFoundException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.BILLING_NOT_FOUND));
    }

    [Fact]
    public async Task Error_Invalid_Request()
    {
        var user = UserBuilder.Build();
        var billing = BillingBuilder.Build(userId: user.Id);
        var request = RequestBillingJsonBuilder.Build();
        request.ServiceName = string.Empty;

        var useCase = CreateUseCase(user, billing);

        var act = async () => await useCase.Execute(billing.Id, request);

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.SERVICE_NAME_REQUIRED));
    }

    private static UpdateBillingUseCase CreateUseCase(User user, Billing? billing)
        => new(
            new BillingsUpdateOnlyRepositoryBuilder().GetById(billing).Build(),
            LoggedUserBuilder.Build(user),
            UnitOfWorkBuilder.Build(),
            MapperBuilder.Build());
}
