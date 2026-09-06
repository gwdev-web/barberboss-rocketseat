using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Security.Tokens;

public record AccessToken(string Token, DateTime ExpiresAt);

public interface IAccessTokenGenerator
{
    AccessToken Generate(User user);
}
