using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Communication.Enums;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace UseCases.Tests.Billings.Register;

public class RegisterBillingUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestBillingJsonBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ClientName.Should().Be(request.ClientName);
        result.ServiceName.Should().Be(request.ServiceName);
        result.Amount.Should().Be(request.Amount);
    }

    [Fact]
    public async Task Error_BarberName_Empty()
    {
        var request = RequestBillingJsonBuilder.Build();
        request.BarberName = string.Empty;

        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(request);

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error =>
            error.GetErrors().Count == 1 &&
            error.GetErrors().Contains(ResourceMessagesException.BARBER_NAME_REQUIRED));
    }

    [Fact]
    public async Task Error_Cancelled_With_Amount()
    {
        var request = RequestBillingJsonBuilder.Build(BillingStatus.Cancelled);
        request.Amount = 120;

        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(request);

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error =>
            error.GetErrors().Contains(ResourceMessagesException.CANCELLED_BILLING_MUST_HAVE_ZERO_AMOUNT));
    }

    private static RegisterBillingUseCase CreateUseCase()
    {
        var repository = new BillingsWriteOnlyRepositoryBuilder().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var mapper = MapperBuilder.Build();

        return new RegisterBillingUseCase(repository, unitOfWork, mapper);
    }
}
