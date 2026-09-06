using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Services.LoggedUser;

public interface ILoggedUser
{
    /// <summary>Usuário dono do token enviado na requisição.</summary>
    Task<User> Get();
}
