using BarberBoss.Domain.Dtos;
using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingsReadOnlyRepository
{
    Task<PagedResultDto<Billing>> GetAll(FilterBillingsDto filter);

    Task<Billing?> GetById(Guid id);

    Task<IList<Billing>> FilterByPeriod(DateOnly startDate, DateOnly endDate);

    Task<BillingsSummaryDto> GetSummary(DateOnly startDate, DateOnly endDate);
}
