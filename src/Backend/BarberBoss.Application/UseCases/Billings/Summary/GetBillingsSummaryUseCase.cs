using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Summary;

/// <summary>
/// Total do período somando apenas os faturamentos com status Pago.
/// Sem datas informadas, considera a semana corrente (segunda a domingo).
/// </summary>
public class GetBillingsSummaryUseCase : IGetBillingsSummaryUseCase
{
    private readonly IBillingsReadOnlyRepository _repository;

    public GetBillingsSummaryUseCase(IBillingsReadOnlyRepository repository) => _repository = repository;

    public async Task<ResponseBillingsSummaryJson> Execute(DateOnly? startDate, DateOnly? endDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var start = startDate ?? today.StartOfWeek();
        var end = endDate ?? start.EndOfWeek();

        if (start > end)
            throw new ErrorOnValidationException([ResourceMessagesException.INVALID_PERIOD]);

        var summary = await _repository.GetSummary(start, end);

        return new ResponseBillingsSummaryJson
        {
            StartDate = start,
            EndDate = end,
            Total = summary.Total,
            PaidCount = summary.PaidCount,
            CancelledCount = summary.CancelledCount,
            AverageTicket = summary.PaidCount == 0
                ? 0
                : Math.Round(summary.Total / summary.PaidCount, 2),
        };
    }
}
