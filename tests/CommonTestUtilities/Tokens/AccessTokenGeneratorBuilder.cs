using BarberBoss.Domain.Security.Tokens;
using BarberBoss.Infrastructure.Security.Tokens;

namespace CommonTestUtilities.Tokens;

public static class AccessTokenGeneratorBuilder
{
    public const string SIGNING_KEY = "chave-de-teste-do-barberboss-com-mais-de-32-caracteres";

    public static IAccessTokenGenerator Build()
        => new JwtTokenGenerator(new JwtSettings
        {
            SigningKey = SIGNING_KEY,
            ExpirationTimeMinutes = 60,
            Issuer = "BarberBoss",
            Audience = "BarberBossClient",
        });
}
