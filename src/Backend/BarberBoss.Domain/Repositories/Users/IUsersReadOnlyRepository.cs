using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Users;

public interface IUsersReadOnlyRepository
{
    Task<bool> ExistsUserWithEmail(string email);

    Task<User?> GetByEmail(string email);

    Task<User?> GetById(Guid id);
}
