using BarberBoss.Domain.Dtos;
using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

/// <summary>
/// Todas as consultas são escopadas pelo usuário logado: ninguém enxerga o faturamento de outro.
/// </summary>
public interface IBillingsReadOnlyRepository
{
    Task<PagedResultDto<Billing>> GetAll(Guid userId, FilterBillingsDto filter);

    Task<Billing?> GetById(Guid userId, Guid id);

    Task<IList<Billing>> FilterByPeriod(Guid userId, DateOnly startDate, DateOnly endDate);

    Task<BillingsSummaryDto> GetSummary(Guid userId, DateOnly startDate, DateOnly endDate);
}
