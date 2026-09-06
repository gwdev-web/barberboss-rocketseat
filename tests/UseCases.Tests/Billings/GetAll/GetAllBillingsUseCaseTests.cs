using BarberBoss.Application.UseCases.Billings.GetAll;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Tests.Billings.GetAll;

public class GetAllBillingsUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var billings = BillingBuilder.Collection(7);
        var useCase = CreateUseCase(billings);

        var result = await useCase.Execute(RequestBillingFilterJsonBuilder.Build(pageNumber: 1, pageSize: 10));

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(7);
        result.TotalItems.Should().Be(7);
        result.TotalPages.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Error_Invalid_PageSize()
    {
        var useCase = CreateUseCase(BillingBuilder.Collection());

        var act = async () => await useCase.Execute(RequestBillingFilterJsonBuilder.Build(pageSize: 500));

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.PAGE_SIZE_INVALID));
    }

    private static GetAllBillingsUseCase CreateUseCase(IList<BarberBoss.Domain.Entities.Billing> billings)
    {
        var repository = new BillingsReadOnlyRepositoryBuilder().GetAll(billings).Build();

        return new GetAllBillingsUseCase(repository, MapperBuilder.Build());
    }
}
