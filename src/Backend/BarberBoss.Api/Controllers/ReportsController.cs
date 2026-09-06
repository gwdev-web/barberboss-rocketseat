using BarberBoss.Application.UseCases.Billings.Reports.Excel;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace BarberBoss.Api.Controllers;

[Route("api/reports")]
[ApiController]
public class ReportsController : ControllerBase
{
    /// <summary>Relatório semanal de faturamento em PDF.</summary>
    /// <param name="referenceDate">
    /// Qualquer data dentro da semana desejada. Sem valor, usa a semana corrente.
    /// </param>
    [HttpGet("pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetPdf(
        [FromServices] IGenerateBillingsReportPdfUseCase useCase,
        [FromQuery] DateOnly? referenceDate)
    {
        var report = await useCase.Execute(referenceDate);

        if (report.IsEmpty)
            return NoContent();

        return File(report.Content, report.ContentType, report.FileName);
    }

    /// <summary>Relatório semanal de faturamento em Excel.</summary>
    [HttpGet("excel")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetExcel(
        [FromServices] IGenerateBillingsReportExcelUseCase useCase,
        [FromQuery] DateOnly? referenceDate)
    {
        var report = await useCase.Execute(referenceDate);

        if (report.IsEmpty)
            return NoContent();

        return File(report.Content, report.ContentType, report.FileName);
    }
}
