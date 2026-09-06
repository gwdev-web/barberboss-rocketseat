namespace BarberBoss.Infrastructure.Security.Tokens;

public class JwtSettings
{
    public const string SECTION = "Settings:Jwt";

    /// <summary>Chave HMAC. Precisa de pelo menos 32 caracteres e nunca deve ir para o repositório.</summary>
    public string SigningKey { get; set; } = string.Empty;

    public int ExpirationTimeMinutes { get; set; } = 120;
    public string Issuer { get; set; } = "BarberBoss";
    public string Audience { get; set; } = "BarberBossClient";
}
