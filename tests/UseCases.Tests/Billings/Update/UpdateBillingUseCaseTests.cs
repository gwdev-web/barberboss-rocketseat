using BarberBoss.Application.UseCases.Billings.Update;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
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
        var billing = BillingBuilder.Build();
        var request = RequestBillingJsonBuilder.Build();

        var useCase = CreateUseCase(billing);

        var act = async () => await useCase.Execute(billing.Id, request);

        await act.Should().NotThrowAsync();

        billing.ClientName.Should().Be(request.ClientName);
        billing.ServiceName.Should().Be(request.ServiceName);
        billing.Amount.Should().Be(request.Amount);
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var request = RequestBillingJsonBuilder.Build();
        var useCase = CreateUseCase(null);

        var act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await act.Should().ThrowAsync<NotFoundException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.BILLING_NOT_FOUND));
    }

    [Fact]
    public async Task Error_Invalid_Request()
    {
        var billing = BillingBuilder.Build();
        var request = RequestBillingJsonBuilder.Build();
        request.ServiceName = string.Empty;

        var useCase = CreateUseCase(billing);

        var act = async () => await useCase.Execute(billing.Id, request);

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error =>
            error.GetErrors().Count == 1 &&
            error.GetErrors().Contains(ResourceMessagesException.SERVICE_NAME_REQUIRED));
    }

    private static UpdateBillingUseCase CreateUseCase(BarberBoss.Domain.Entities.Billing? billing)
    {
        var repository = new BillingsUpdateOnlyRepositoryBuilder().GetById(billing).Build();

        return new UpdateBillingUseCase(repository, UnitOfWorkBuilder.Build(), MapperBuilder.Build());
    }
}
