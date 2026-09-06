namespace BarberBoss.Communication.Responses;

public class ResponseBillingsSummaryJson
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    /// <summary>Soma dos faturamentos com status Pago. Cancelados não entram no total.</summary>
    public decimal Total { get; set; }

    public int PaidCount { get; set; }
    public int CancelledCount { get; set; }
    public decimal AverageTicket { get; set; }
}
