using BarberBoss.Communication.Enums;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
using ClosedXML.Excel;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public class GenerateBillingsReportExcelUseCase : IGenerateBillingsReportExcelUseCase
{
    private const string CURRENCY_FORMAT = "R$ #,##0.00";
    private const string DATE_FORMAT = "dd/MM/yyyy";
    private const string DARK = "#171717";
    private const string GOLD = "#F5B301";

    private readonly IBillingsReadOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;

    public GenerateBillingsReportExcelUseCase(IBillingsReadOnlyRepository repository, ILoggedUser loggedUser)
    {
        _repository = repository;
        _loggedUser = loggedUser;
    }

    public async Task<ReportFile> Execute(DateOnly? referenceDate)
    {
        var (start, end) = ReportPeriod.Resolve(referenceDate);

        var loggedUser = await _loggedUser.Get();

        var billings = await _repository.FilterByPeriod(loggedUser.Id, start, end);

        var fileName = ReportFileNames.Excel(start, end);

        if (billings.Count == 0)
            return new ReportFile([], fileName, ReportFileNames.EXCEL_CONTENT_TYPE);

        var content = Build(billings, start, end);

        return new ReportFile(content, fileName, ReportFileNames.EXCEL_CONTENT_TYPE);
    }

    private static byte[] Build(IList<Billing> billings, DateOnly start, DateOnly end)
    {
        using var workbook = new XLWorkbook();

        workbook.Author = "BarberBoss";
        workbook.Style.Font.FontSize = 11;
        workbook.Style.Font.FontName = "Calibri";

        var worksheet = workbook.Worksheets.Add("Faturamento");

        // ----- Cabeçalho -----
        worksheet.Cell("A1").Value = "BARBER BOSS";
        worksheet.Range("A1:G1").Merge();
        worksheet.Cell("A1").Style.Font.FontSize = 18;
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.FromHtml(GOLD);
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml(DARK);
        worksheet.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Row(1).Height = 30;

        worksheet.Cell("A2").Value = $"Relatório semanal · {start:dd/MM/yyyy} a {end:dd/MM/yyyy}";
        worksheet.Range("A2:G2").Merge();
        worksheet.Cell("A2").Style.Font.FontColor = XLColor.FromHtml("#FFFFFF");
        worksheet.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromHtml(DARK);

        // ----- Colunas -----
        var headers = new[] { "Data", "Barbeiro", "Cliente", "Serviço", "Pagamento", "Status", "Valor" };

        for (var column = 0; column < headers.Length; column++)
        {
            var cell = worksheet.Cell(4, column + 1);
            cell.Value = headers[column];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.FromHtml("#FFFFFF");
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#3F3F46");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // ----- Linhas -----
        var row = 5;

        foreach (var billing in billings.OrderBy(billing => billing.Date).ThenBy(billing => billing.CreatedAt))
        {
            worksheet.Cell(row, 1).Value = billing.Date.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell(row, 1).Style.DateFormat.Format = DATE_FORMAT;

            worksheet.Cell(row, 2).Value = billing.BarberName;
            worksheet.Cell(row, 3).Value = billing.ClientName;
            worksheet.Cell(row, 4).Value = billing.ServiceName;
            worksheet.Cell(row, 5).Value = billing.PaymentMethod.GetDescription();
            worksheet.Cell(row, 6).Value = billing.Status.GetDescription();

            worksheet.Cell(row, 7).Value = billing.Amount;
            worksheet.Cell(row, 7).Style.NumberFormat.Format = CURRENCY_FORMAT;

            if (billing.Status == BillingStatus.Cancelled)
                worksheet.Range(row, 1, row, 7).Style.Font.FontColor = XLColor.FromHtml("#9CA3AF");

            row++;
        }

        // ----- Totalizadores -----
        var paid = billings.Where(billing => billing.Status == BillingStatus.Paid).ToList();
        var total = paid.Sum(billing => billing.Amount);

        var totalRow = row + 1;

        worksheet.Cell(totalRow, 6).Value = "TOTAL PAGO";
        worksheet.Cell(totalRow, 6).Style.Font.Bold = true;
        worksheet.Cell(totalRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        worksheet.Cell(totalRow, 7).Value = total;
        worksheet.Cell(totalRow, 7).Style.NumberFormat.Format = CURRENCY_FORMAT;
        worksheet.Cell(totalRow, 7).Style.Font.Bold = true;
        worksheet.Range(totalRow, 1, totalRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#F4F4F5");

        worksheet.Cell(totalRow + 1, 6).Value = "Ticket médio";
        worksheet.Cell(totalRow + 1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        worksheet.Cell(totalRow + 1, 7).Value = paid.Count == 0 ? 0 : Math.Round(total / paid.Count, 2);
        worksheet.Cell(totalRow + 1, 7).Style.NumberFormat.Format = CURRENCY_FORMAT;

        worksheet.Cell(totalRow + 2, 6).Value = "Atendimentos pagos";
        worksheet.Cell(totalRow + 2, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        worksheet.Cell(totalRow + 2, 7).Value = paid.Count;

        worksheet.Cell(totalRow + 3, 6).Value = "Cancelados";
        worksheet.Cell(totalRow + 3, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        worksheet.Cell(totalRow + 3, 7).Value = billings.Count - paid.Count;

        worksheet.SheetView.FreezeRows(4);
        worksheet.Columns().AdjustToContents();

        using var file = new MemoryStream();
        workbook.SaveAs(file);

        return file.ToArray();
    }
}
