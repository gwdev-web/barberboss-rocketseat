using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Infrastructure.Security.Cryptography;

namespace CommonTestUtilities.Cryptography;

public static class PasswordEncripterBuilder
{
    /// <summary>Usa a implementação real (BCrypt) para os testes cobrirem o hash de verdade.</summary>
    public static IPasswordEncripter Build() => new PasswordEncripter();
}
