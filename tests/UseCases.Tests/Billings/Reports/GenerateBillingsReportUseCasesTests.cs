using BarberBoss.Application.UseCases.Billings.Reports.Excel;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf;
using BarberBoss.Communication.Enums;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using QuestPDF.Infrastructure;

namespace UseCases.Tests.Billings.Reports;

public class GenerateBillingsReportUseCasesTests
{
    public GenerateBillingsReportUseCasesTests() => QuestPDF.Settings.License = LicenseType.Community;

    [Fact]
    public async Task Pdf_Success()
    {
        var billings = new List<BarberBoss.Domain.Entities.Billing>
        {
            BillingBuilder.Build(BillingStatus.Paid, new DateOnly(2025, 3, 3)),
            BillingBuilder.Build(BillingStatus.Cancelled, new DateOnly(2025, 3, 4)),
        };

        var repository = new BillingsReadOnlyRepositoryBuilder().FilterByPeriod(billings).Build();
        var useCase = new GenerateBillingsReportPdfUseCase(repository, LoggedUserBuilder.Build(UserBuilder.Build()));

        var report = await useCase.Execute(new DateOnly(2025, 3, 5));

        report.IsEmpty.Should().BeFalse();
        report.Content.Should().NotBeEmpty();
        report.FileName.Should().EndWith(".pdf");
    }

    [Fact]
    public async Task Pdf_Returns_Empty_When_There_Is_No_Billing()
    {
        var repository = new BillingsReadOnlyRepositoryBuilder().FilterByPeriod([]).Build();
        var useCase = new GenerateBillingsReportPdfUseCase(repository, LoggedUserBuilder.Build(UserBuilder.Build()));

        var report = await useCase.Execute(referenceDate: null);

        report.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public async Task Excel_Success()
    {
        var billings = BillingBuilder.Collection(4);

        var repository = new BillingsReadOnlyRepositoryBuilder().FilterByPeriod(billings).Build();
        var useCase = new GenerateBillingsReportExcelUseCase(repository, LoggedUserBuilder.Build(UserBuilder.Build()));

        var report = await useCase.Execute(new DateOnly(2025, 3, 5));

        report.IsEmpty.Should().BeFalse();
        report.Content.Should().NotBeEmpty();
        report.FileName.Should().EndWith(".xlsx");
    }

    [Fact]
    public async Task Excel_Returns_Empty_When_There_Is_No_Billing()
    {
        var repository = new BillingsReadOnlyRepositoryBuilder().FilterByPeriod([]).Build();
        var useCase = new GenerateBillingsReportExcelUseCase(repository, LoggedUserBuilder.Build(UserBuilder.Build()));

        var report = await useCase.Execute(referenceDate: null);

        report.IsEmpty.Should().BeTrue();
    }
}
