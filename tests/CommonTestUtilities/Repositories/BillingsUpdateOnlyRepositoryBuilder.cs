using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Moq;

namespace CommonTestUtilities.Repositories;

public class BillingsUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IBillingsUpdateOnlyRepository> _repository = new();

    public BillingsUpdateOnlyRepositoryBuilder GetById(Billing? billing)
    {
        _repository
            .Setup(repository => repository.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(billing);

        return this;
    }

    public IBillingsUpdateOnlyRepository Build() => _repository.Object;
}
