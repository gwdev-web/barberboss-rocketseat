using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingsWriteOnlyRepository
{
    Task Add(Billing billing);

    /// <returns><c>true</c> quando o faturamento existia, pertencia ao usuário e foi removido.</returns>
    Task<bool> Delete(Guid userId, Guid id);
}
