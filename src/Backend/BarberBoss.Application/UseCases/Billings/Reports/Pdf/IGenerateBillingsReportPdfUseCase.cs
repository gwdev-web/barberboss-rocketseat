using BarberBoss.Domain.Reports;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;

public interface IGenerateBillingsReportPdfUseCase
{
    Task<ReportFile> Execute(DateOnly? referenceDate);
}
