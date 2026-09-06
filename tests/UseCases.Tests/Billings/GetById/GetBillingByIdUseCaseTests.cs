using BarberBoss.Application.UseCases.Billings.GetById;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Tests.Billings.GetById;

public class GetBillingByIdUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var billing = BillingBuilder.Build(userId: user.Id);

        var useCase = new GetBillingByIdUseCase(
            new BillingsReadOnlyRepositoryBuilder().GetById(billing).Build(),
            LoggedUserBuilder.Build(user),
            MapperBuilder.Build());

        var result = await useCase.Execute(billing.Id);

        result.Id.Should().Be(billing.Id);
        result.ClientName.Should().Be(billing.ClientName);
    }

    [Fact]
    public async Task Error_Billing_From_Another_User_Is_Not_Found()
    {
        var user = UserBuilder.Build();

        // O repositório é escopado pelo usuário: faturamento de terceiro volta como null.
        var useCase = new GetBillingByIdUseCase(
            new BillingsReadOnlyRepositoryBuilder().GetById(null).Build(),
            LoggedUserBuilder.Build(user),
            MapperBuilder.Build());

        var act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.Should().ThrowAsync<NotFoundException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.BILLING_NOT_FOUND));
    }
}
