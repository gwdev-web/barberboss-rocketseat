using System.ComponentModel;

namespace BarberBoss.Communication.Enums;

public enum UserRole
{
    [Description("Usuário")]
    User = 0,

    [Description("Administrador")]
    Admin = 1,
}
