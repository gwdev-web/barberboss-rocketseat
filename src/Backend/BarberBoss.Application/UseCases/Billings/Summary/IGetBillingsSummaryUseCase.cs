using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Billings.Summary;

public interface IGetBillingsSummaryUseCase
{
    Task<ResponseBillingsSummaryJson> Execute(DateOnly? startDate, DateOnly? endDate);
}
