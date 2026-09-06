using System.Globalization;
using BarberBoss.Communication.Enums;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories.Billings;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;

public class GenerateBillingsReportPdfUseCase : IGenerateBillingsReportPdfUseCase
{
    private static readonly CultureInfo Culture = new("pt-BR");

    private readonly IBillingsReadOnlyRepository _repository;

    public GenerateBillingsReportPdfUseCase(IBillingsReadOnlyRepository repository) => _repository = repository;

    public async Task<ReportFile> Execute(DateOnly? referenceDate)
    {
        var (start, end) = ReportPeriod.Resolve(referenceDate);

        var billings = await _repository.FilterByPeriod(start, end);

        var fileName = ReportFileNames.Pdf(start, end);

        if (billings.Count == 0)
            return new ReportFile([], fileName, ReportFileNames.PDF_CONTENT_TYPE);

        var content = Build(billings, start, end);

        return new ReportFile(content, fileName, ReportFileNames.PDF_CONTENT_TYPE);
    }

    private static byte[] Build(IList<Billing> billings, DateOnly start, DateOnly end)
    {
        var paid = billings.Where(billing => billing.Status == BillingStatus.Paid).ToList();
        var total = paid.Sum(billing => billing.Amount);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(style => style.FontSize(9).FontColor(Colors.Grey.Darken4));

                page.Header().Element(header => ComposeHeader(header, start, end));
                page.Content().Element(content => ComposeContent(content, billings, paid.Count, total));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, DateOnly start, DateOnly end)
    {
        container.PaddingBottom(16).Column(column =>
        {
            column.Item()
                .Background(Colors.Grey.Darken4)
                .Padding(16)
                .Row(row =>
                {
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text("BARBER BOSS")
                            .FontSize(20).Bold().FontColor(Colors.Amber.Medium);

                        inner.Item().Text("Relatório semanal de faturamento")
                            .FontSize(10).FontColor(Colors.Grey.Lighten2);
                    });

                    row.ConstantItem(170).AlignRight().Column(inner =>
                    {
                        inner.Item().Text("Período").FontSize(8).FontColor(Colors.Grey.Lighten1);
                        inner.Item().Text($"{start:dd/MM/yyyy} a {end:dd/MM/yyyy}")
                            .FontSize(11).SemiBold().FontColor(Colors.White);
                    });
                });
        });
    }

    private static void ComposeContent(IContainer container, IList<Billing> billings, int paidCount, decimal total)
    {
        container.Column(column =>
        {
            column.Spacing(14);

            column.Item().Row(row =>
            {
                row.Spacing(10);
                row.RelativeItem().Element(item => Metric(item, "Total do período (pagos)", Money(total)));
                row.RelativeItem().Element(item => Metric(item, "Atendimentos pagos", paidCount.ToString()));
                row.RelativeItem().Element(item => Metric(item, "Cancelados", (billings.Count - paidCount).ToString()));
                row.RelativeItem().Element(item => Metric(item, "Ticket médio",
                    Money(paidCount == 0 ? 0 : Math.Round(total / paidCount, 2))));
            });

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(62);   // Data
                    columns.RelativeColumn(2);    // Barbeiro
                    columns.RelativeColumn(2);    // Cliente
                    columns.RelativeColumn(2);    // Serviço
                    columns.ConstantColumn(62);   // Pagamento
                    columns.ConstantColumn(58);   // Status
                    columns.ConstantColumn(72);   // Valor
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Data");
                    header.Cell().Element(HeaderCell).Text("Barbeiro");
                    header.Cell().Element(HeaderCell).Text("Cliente");
                    header.Cell().Element(HeaderCell).Text("Serviço");
                    header.Cell().Element(HeaderCell).Text("Pagamento");
                    header.Cell().Element(HeaderCell).Text("Status");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Valor");
                });

                foreach (var billing in billings.OrderBy(billing => billing.Date).ThenBy(billing => billing.CreatedAt))
                {
                    table.Cell().Element(BodyCell).Text($"{billing.Date:dd/MM}");
                    table.Cell().Element(BodyCell).Text(billing.BarberName);
                    table.Cell().Element(BodyCell).Text(billing.ClientName);
                    table.Cell().Element(BodyCell).Text(billing.ServiceName);
                    table.Cell().Element(BodyCell).Text(billing.PaymentMethod.GetDescription());
                    table.Cell().Element(BodyCell).Text(billing.Status.GetDescription());
                    table.Cell().Element(BodyCell).AlignRight().Text(Money(billing.Amount));
                }

                table.Cell().ColumnSpan(6).Element(TotalCell).AlignRight().Text("TOTAL PAGO").Bold();
                table.Cell().Element(TotalCell).AlignRight().Text(Money(total)).Bold();
            });
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.PaddingTop(10).Row(row =>
        {
            row.RelativeItem()
                .Text($"Gerado em {DateTime.Now.ToString("dd/MM/yyyy HH:mm", Culture)}")
                .FontSize(8).FontColor(Colors.Grey.Darken1);

            row.RelativeItem().AlignRight().Text(text =>
            {
                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Darken1));
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
            });
        });
    }

    private static void Metric(IContainer container, string label, string value)
    {
        container
            .Background(Colors.Grey.Lighten4)
            .BorderLeft(3)
            .BorderColor(Colors.Amber.Medium)
            .Padding(10)
            .Column(column =>
            {
                column.Item().Text(label).FontSize(7).FontColor(Colors.Grey.Darken2);
                column.Item().PaddingTop(3).Text(value).FontSize(12).Bold();
            });
    }

    private static IContainer HeaderCell(IContainer container) => container
        .Background(Colors.Grey.Darken4)
        .PaddingVertical(6)
        .PaddingHorizontal(5)
        .DefaultTextStyle(style => style.FontColor(Colors.White).SemiBold().FontSize(8));

    private static IContainer BodyCell(IContainer container) => container
        .BorderBottom(1)
        .BorderColor(Colors.Grey.Lighten2)
        .PaddingVertical(5)
        .PaddingHorizontal(5);

    private static IContainer TotalCell(IContainer container) => container
        .Background(Colors.Grey.Lighten3)
        .PaddingVertical(8)
        .PaddingHorizontal(5);

    private static string Money(decimal value) => $"R$ {value.ToString("N2", Culture)}";
}
