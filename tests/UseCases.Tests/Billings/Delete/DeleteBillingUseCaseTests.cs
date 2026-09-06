using BarberBoss.Application.UseCases.Billings.Delete;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Tests.Billings.Delete;

public class DeleteBillingUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var id = Guid.NewGuid();
        var repository = new BillingsWriteOnlyRepositoryBuilder().Delete(id, result: true).Build();
        var useCase = new DeleteBillingUseCase(repository, UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var id = Guid.NewGuid();
        var repository = new BillingsWriteOnlyRepositoryBuilder().Delete(id, result: false).Build();
        var useCase = new DeleteBillingUseCase(repository, UnitOfWorkBuilder.Build());

        var act = async () => await useCase.Execute(id);

        var exception = await act.Should().ThrowAsync<NotFoundException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.BILLING_NOT_FOUND));
    }
}
