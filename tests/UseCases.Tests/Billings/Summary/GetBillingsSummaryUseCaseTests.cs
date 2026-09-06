using BarberBoss.Application.UseCases.Billings.Summary;
using BarberBoss.Domain.Dtos;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Repositories;
using FluentAssertions;

namespace UseCases.Tests.Billings.Summary;

public class GetBillingsSummaryUseCaseTests
{
    [Fact]
    public async Task Success_Total_Only_Counts_Paid_Billings()
    {
        var summary = new BillingsSummaryDto { Total = 450m, PaidCount = 6, CancelledCount = 2 };

        var repository = new BillingsReadOnlyRepositoryBuilder().GetSummary(summary).Build();
        var useCase = new GetBillingsSummaryUseCase(repository);

        var result = await useCase.Execute(new DateOnly(2025, 3, 3), new DateOnly(2025, 3, 9));

        result.Total.Should().Be(450m);
        result.PaidCount.Should().Be(6);
        result.CancelledCount.Should().Be(2);
        result.AverageTicket.Should().Be(75m);
    }

    [Fact]
    public async Task Success_Average_Is_Zero_When_There_Is_No_Paid_Billing()
    {
        var summary = new BillingsSummaryDto { Total = 0m, PaidCount = 0, CancelledCount = 3 };

        var repository = new BillingsReadOnlyRepositoryBuilder().GetSummary(summary).Build();
        var useCase = new GetBillingsSummaryUseCase(repository);

        var result = await useCase.Execute(null, null);

        result.Total.Should().Be(0m);
        result.AverageTicket.Should().Be(0m);
    }

    [Fact]
    public async Task Success_Defaults_To_Current_Week()
    {
        var summary = new BillingsSummaryDto { Total = 100m, PaidCount = 1, CancelledCount = 0 };

        var repository = new BillingsReadOnlyRepositoryBuilder().GetSummary(summary).Build();
        var useCase = new GetBillingsSummaryUseCase(repository);

        var result = await useCase.Execute(null, null);

        result.StartDate.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.EndDate.DayOfWeek.Should().Be(DayOfWeek.Sunday);
        result.EndDate.DayNumber.Should().Be(result.StartDate.DayNumber + 6);
    }

    [Fact]
    public async Task Error_Invalid_Period()
    {
        var summary = new BillingsSummaryDto();
        var repository = new BillingsReadOnlyRepositoryBuilder().GetSummary(summary).Build();
        var useCase = new GetBillingsSummaryUseCase(repository);

        var act = async () => await useCase.Execute(new DateOnly(2025, 3, 10), new DateOnly(2025, 3, 1));

        var exception = await act.Should().ThrowAsync<ErrorOnValidationException>();

        exception.Where(error => error.GetErrors().Contains(ResourceMessagesException.INVALID_PERIOD));
    }
}
