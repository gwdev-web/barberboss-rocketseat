using BarberBoss.Application.UseCases.Billings.GetById;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Tests.Billings.GetById;

public class GetBillingByIdUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var billing = BillingBuilder.Build();
        var repository = new BillingsReadOnlyRepositoryBuilder().GetById(billing).Build();
        var useCase = new GetBillingByIdUseCase(repository, MapperBuilder.Build());

        var result = await useCase.Execute(billing.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(billing.Id);
        result.ClientName.Should().Be(billing.ClientName);
        result.Amount.Should().Be(billing.Amount);
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var repository = new BillingsReadOnlyRepositoryBuilder().GetById(null).Build();
        var useCase = new GetBillingByIdUseCase(repository, MapperBuilder.Build());

        var act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.Should().ThrowAsync<NotFoundException>();

        exception.Where(error =>
            error.GetErrors().Count == 1 &&
            error.GetErrors().Contains(ResourceMessagesException.BILLING_NOT_FOUND));
    }
}
