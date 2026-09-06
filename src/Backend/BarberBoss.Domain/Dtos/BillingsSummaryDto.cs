namespace BarberBoss.Domain.Dtos;

public class BillingsSummaryDto
{
    public decimal Total { get; set; }
    public int PaidCount { get; set; }
    public int CancelledCount { get; set; }
}
