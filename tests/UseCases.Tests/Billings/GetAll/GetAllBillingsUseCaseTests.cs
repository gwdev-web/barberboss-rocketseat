using BarberBoss.Application.UseCases.Billings.GetAll;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
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
        var user = UserBuilder.Build();
        var billings = BillingBuilder.Collection(7, user.Id);

        var useCase = new GetAllBillingsUseCase(
            new BillingsReadOnlyRepositoryBuilder().GetAll(billings).Build(),
            LoggedUserBuilder.Build(user),
            MapperBuilder.Build());

        var result = await useCase.Execute(RequestBillingFilterJsonBuilder.Build(pageNumber: 1, pageSize: 10));

        result.Items.Should().HaveCount(7);
        result.TotalItems.Should().Be(7);
        result.TotalPages.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Error_Invalid_PageSize()
    {
        var user = UserBuilder.Build();

        var useCase = new GetAllBillingsUseCase(
            new BillingsReadOnlyRepositoryBuilder().GetAll(BillingBuilder.Collection()).Build(),
            LoggedUserBuilder.Build(user),
            MapperBuilder.Build());

        var act = async () => await useCase.Execute(RequestBillingFilterJsonBuilder.Build(pageSize: 500));

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.PAGE_SIZE_INVALID));
    }
}
