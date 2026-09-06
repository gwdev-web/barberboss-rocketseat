using BarberBoss.Communication.Enums;

namespace BarberBoss.Domain.Entities;

public class Billing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateOnly Date { get; set; }
    public string BarberName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public BillingStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Dono do registro. Cada usuário só enxerga o próprio faturamento.</summary>
    public Guid UserId { get; set; }

    public User? User { get; set; }
}
