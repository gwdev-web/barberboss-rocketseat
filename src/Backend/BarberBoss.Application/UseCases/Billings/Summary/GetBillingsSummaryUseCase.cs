using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
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
    private readonly ILoggedUser _loggedUser;

    public GetBillingsSummaryUseCase(IBillingsReadOnlyRepository repository, ILoggedUser loggedUser)
    {
        _repository = repository;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseBillingsSummaryJson> Execute(DateOnly? startDate, DateOnly? endDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var start = startDate ?? today.StartOfWeek();
        var end = endDate ?? start.EndOfWeek();

        if (start > end)
            throw new ErrorOnValidationException([ResourceMessagesException.INVALID_PERIOD]);

        var loggedUser = await _loggedUser.Get();

        var summary = await _repository.GetSummary(loggedUser.Id, start, end);

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
