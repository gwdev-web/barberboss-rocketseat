using BarberBoss.Domain.Reports;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public interface IGenerateBillingsReportExcelUseCase
{
    Task<ReportFile> Execute(DateOnly? referenceDate);
}
