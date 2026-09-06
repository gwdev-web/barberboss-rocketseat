using BarberBoss.Application.UseCases.Billings.Delete;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Tests.Billings.Delete;

public class DeleteBillingUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var id = Guid.NewGuid();

        var useCase = new DeleteBillingUseCase(
            new BillingsWriteOnlyRepositoryBuilder().Delete(user.Id, id, result: true).Build(),
            LoggedUserBuilder.Build(user),
            UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var user = UserBuilder.Build();
        var id = Guid.NewGuid();

        var useCase = new DeleteBillingUseCase(
            new BillingsWriteOnlyRepositoryBuilder().Delete(user.Id, id, result: false).Build(),
            LoggedUserBuilder.Build(user),
            UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(id);

        var exception = await act.Should().ThrowAsync<NotFoundException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.BILLING_NOT_FOUND));
    }
}
