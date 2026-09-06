using BarberBoss.Communication.Enums;

namespace BarberBoss.Domain.Dtos;

public class FilterBillingsDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? BarberName { get; set; }
    public string? ClientName { get; set; }
    public string? ServiceName { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public BillingStatus? Status { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public BillingSortBy SortBy { get; set; } = BillingSortBy.Date;
    public SortDirection SortDirection { get; set; } = SortDirection.Desc;
}
