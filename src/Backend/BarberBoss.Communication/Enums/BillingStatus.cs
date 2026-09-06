using System.ComponentModel;

namespace BarberBoss.Communication.Enums;

public enum BillingStatus
{
    [Description("Pago")]
    Paid = 0,

    [Description("Cancelado")]
    Cancelled = 1,
}
