using System.ComponentModel;

namespace BarberBoss.Communication.Enums;

public enum PaymentMethod
{
    [Description("Cartão")]
    Card = 0,

    [Description("Dinheiro")]
    Cash = 1,

    [Description("Pix")]
    Pix = 2,

    [Description("Outro")]
    Other = 3,
}
