using BarberBoss.Domain.Repositories.Billings;
using Moq;

namespace CommonTestUtilities.Repositories;

public class BillingsWriteOnlyRepositoryBuilder
{
    private readonly Mock<IBillingsWriteOnlyRepository> _repository = new();

    public BillingsWriteOnlyRepositoryBuilder Delete(Guid id, bool result)
    {
        _repository.Setup(repository => repository.Delete(id)).ReturnsAsync(result);

        return this;
    }

    public IBillingsWriteOnlyRepository Build() => _repository.Object;
}
