namespace BarberBoss.Domain.Reports;

public static class ReportFileNames
{
    public const string PDF_CONTENT_TYPE = "application/pdf";
    public const string EXCEL_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static string Pdf(DateOnly start, DateOnly end)
        => $"barberboss-faturamento-{start:yyyy-MM-dd}_a_{end:yyyy-MM-dd}.pdf";

    public static string Excel(DateOnly start, DateOnly end)
        => $"barberboss-faturamento-{start:yyyy-MM-dd}_a_{end:yyyy-MM-dd}.xlsx";
}
