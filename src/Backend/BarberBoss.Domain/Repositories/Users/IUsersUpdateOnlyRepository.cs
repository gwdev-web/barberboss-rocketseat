using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Users;

public interface IUsersUpdateOnlyRepository
{
    Task<User?> GetById(Guid id);

    void Update(User user);
}
