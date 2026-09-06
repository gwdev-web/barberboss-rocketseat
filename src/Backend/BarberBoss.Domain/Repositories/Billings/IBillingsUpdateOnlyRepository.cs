using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingsUpdateOnlyRepository
{
    Task<Billing?> GetById(Guid userId, Guid id);

    void Update(Billing billing);
}
