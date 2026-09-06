using BarberBoss.Domain.Security.Cryptography;

namespace BarberBoss.Infrastructure.Security.Cryptography;

/// <summary>
/// BCrypt com salt por usuário. O work factor padrão do pacote (11) já é adequado;
/// aumentá-lo encarece o ataque de força bruta ao custo de um login mais lento.
/// </summary>
public class PasswordEncripter : IPasswordEncripter
{
    private const int WORK_FACTOR = 12;

    public string Encrypt(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: WORK_FACTOR);

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
