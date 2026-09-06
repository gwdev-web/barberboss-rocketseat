using BarberBoss.Communication.Enums;

namespace BarberBoss.Communication.Responses;

public class ResponseShortBillingJson
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string BarberName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BillingStatus Status { get; set; }
}
