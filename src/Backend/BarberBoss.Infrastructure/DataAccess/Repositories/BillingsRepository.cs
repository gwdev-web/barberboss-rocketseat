using BarberBoss.Communication.Enums;
using BarberBoss.Domain.Dtos;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

public class BillingsRepository :
    IBillingsWriteOnlyRepository,
    IBillingsReadOnlyRepository,
    IBillingsUpdateOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public BillingsRepository(BarberBossDbContext dbContext) => _dbContext = dbContext;

    public async Task Add(Billing billing) => await _dbContext.Billings.AddAsync(billing);

    public async Task<bool> Delete(Guid id)
    {
        var billing = await _dbContext.Billings.FirstOrDefaultAsync(entity => entity.Id == id);

        if (billing is null)
            return false;

        _dbContext.Billings.Remove(billing);

        return true;
    }

    public async Task<PagedResultDto<Billing>> GetAll(FilterBillingsDto filter)
    {
        var query = _dbContext.Billings.AsNoTracking().AsQueryable();

        query = ApplyFilters(query, filter);

        var totalItems = await query.CountAsync();

        query = ApplyOrdering(query, filter);

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResultDto<Billing> { Items = items, TotalItems = totalItems };
    }

    async Task<Billing?> IBillingsReadOnlyRepository.GetById(Guid id)
        => await _dbContext.Billings.AsNoTracking().FirstOrDefaultAsync(billing => billing.Id == id);

    async Task<Billing?> IBillingsUpdateOnlyRepository.GetById(Guid id)
        => await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);

    public void Update(Billing billing) => _dbContext.Billings.Update(billing);

    public async Task<IList<Billing>> FilterByPeriod(DateOnly startDate, DateOnly endDate)
    {
        return await _dbContext.Billings
            .AsNoTracking()
            .Where(billing => billing.Date >= startDate && billing.Date <= endDate)
            .OrderBy(billing => billing.Date)
            .ThenBy(billing => billing.CreatedAt)
            .ToListAsync();
    }

    public async Task<BillingsSummaryDto> GetSummary(DateOnly startDate, DateOnly endDate)
    {
        var query = _dbContext.Billings
            .AsNoTracking()
            .Where(billing => billing.Date >= startDate && billing.Date <= endDate);

        // Regra do desafio: apenas faturamentos pagos entram no total.
        var total = await query
            .Where(billing => billing.Status == BillingStatus.Paid)
            .SumAsync(billing => (decimal?)billing.Amount) ?? 0m;

        var paidCount = await query.CountAsync(billing => billing.Status == BillingStatus.Paid);
        var cancelledCount = await query.CountAsync(billing => billing.Status == BillingStatus.Cancelled);

        return new BillingsSummaryDto
        {
            Total = total,
            PaidCount = paidCount,
            CancelledCount = cancelledCount,
        };
    }

    private static IQueryable<Billing> ApplyFilters(IQueryable<Billing> query, FilterBillingsDto filter)
    {
        if (filter.StartDate.HasValue)
            query = query.Where(billing => billing.Date >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(billing => billing.Date <= filter.EndDate.Value);

        if (string.IsNullOrWhiteSpace(filter.BarberName) == false)
            query = query.Where(billing => billing.BarberName.Contains(filter.BarberName));

        if (string.IsNullOrWhiteSpace(filter.ClientName) == false)
            query = query.Where(billing => billing.ClientName.Contains(filter.ClientName));

        if (string.IsNullOrWhiteSpace(filter.ServiceName) == false)
            query = query.Where(billing => billing.ServiceName.Contains(filter.ServiceName));

        if (filter.PaymentMethod.HasValue)
            query = query.Where(billing => billing.PaymentMethod == filter.PaymentMethod.Value);

        if (filter.Status.HasValue)
            query = query.Where(billing => billing.Status == filter.Status.Value);

        return query;
    }

    private static IQueryable<Billing> ApplyOrdering(IQueryable<Billing> query, FilterBillingsDto filter)
    {
        var descending = filter.SortDirection == SortDirection.Desc;

        return filter.SortBy switch
        {
            BillingSortBy.Amount => descending
                ? query.OrderByDescending(billing => billing.Amount)
                : query.OrderBy(billing => billing.Amount),

            BillingSortBy.BarberName => descending
                ? query.OrderByDescending(billing => billing.BarberName)
                : query.OrderBy(billing => billing.BarberName),

            BillingSortBy.ClientName => descending
                ? query.OrderByDescending(billing => billing.ClientName)
                : query.OrderBy(billing => billing.ClientName),

            BillingSortBy.CreatedAt => descending
                ? query.OrderByDescending(billing => billing.CreatedAt)
                : query.OrderBy(billing => billing.CreatedAt),

            _ => descending
                ? query.OrderByDescending(billing => billing.Date).ThenByDescending(billing => billing.CreatedAt)
                : query.OrderBy(billing => billing.Date).ThenBy(billing => billing.CreatedAt),
        };
    }
}
