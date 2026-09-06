using BarberBoss.Communication.Enums;

namespace BarberBoss.Communication.Requests;

public class RequestBillingJson
{
    /// <summary>Data em que o serviço foi faturado. Formato: yyyy-MM-dd.</summary>
    /// <example>2025-01-15</example>
    public DateOnly Date { get; set; }

    /// <summary>Nome do barbeiro responsável (2 a 80 caracteres).</summary>
    /// <example>Rafael Souza</example>
    public string BarberName { get; set; } = string.Empty;

    /// <summary>Nome do cliente atendido (2 a 120 caracteres).</summary>
    /// <example>João Pedro</example>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>Serviço prestado (2 a 120 caracteres).</summary>
    /// <example>Corte + Barba</example>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>Valor cobrado. Deve ser 0 quando o status for Cancelado.</summary>
    /// <example>75.00</example>
    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public BillingStatus Status { get; set; }

    /// <summary>Observações livres (até 500 caracteres).</summary>
    public string? Notes { get; set; }
}
