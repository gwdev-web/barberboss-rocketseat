using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

public class UsersRepository :
    IUsersWriteOnlyRepository,
    IUsersReadOnlyRepository,
    IUsersUpdateOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public UsersRepository(BarberBossDbContext dbContext) => _dbContext = dbContext;

    public async Task Add(User user) => await _dbContext.Users.AddAsync(user);

    public async Task<bool> ExistsUserWithEmail(string email)
        => await _dbContext.Users.AnyAsync(user => user.Email == email);

    public async Task<User?> GetByEmail(string email)
        => await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == email);

    async Task<User?> IUsersReadOnlyRepository.GetById(Guid id)
        => await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id);

    async Task<User?> IUsersUpdateOnlyRepository.GetById(Guid id)
        => await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);

    public void Update(User user) => _dbContext.Users.Update(user);

    public async Task<bool> Delete(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(entity => entity.Id == id);

        if (user is null)
            return false;

        // Os faturamentos do usuário saem junto (cascade configurado no DbContext).
        _dbContext.Users.Remove(user);

        return true;
    }
}
