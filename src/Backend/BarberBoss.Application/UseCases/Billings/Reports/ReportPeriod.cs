using BarberBoss.Domain.Extensions;

namespace BarberBoss.Application.UseCases.Billings.Reports;

/// <summary>
/// Resolve o intervalo do relatório. O desafio pede o total da semana, então quando
/// nenhuma data é informada usamos a semana corrente (segunda a domingo).
/// </summary>
public static class ReportPeriod
{
    public static (DateOnly Start, DateOnly End) Resolve(DateOnly? referenceDate)
    {
        var reference = referenceDate ?? DateOnly.FromDateTime(DateTime.Today);

        return (reference.StartOfWeek(), reference.EndOfWeek());
    }
}
