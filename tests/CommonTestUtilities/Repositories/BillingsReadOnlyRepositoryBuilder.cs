using BarberBoss.Domain.Dtos;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Moq;

namespace CommonTestUtilities.Repositories;

public class BillingsReadOnlyRepositoryBuilder
{
    private readonly Mock<IBillingsReadOnlyRepository> _repository = new();

    public BillingsReadOnlyRepositoryBuilder GetAll(IList<Billing> billings)
    {
        _repository
            .Setup(repository => repository.GetAll(It.IsAny<FilterBillingsDto>()))
            .ReturnsAsync(new PagedResultDto<Billing> { Items = billings, TotalItems = billings.Count });

        return this;
    }

    public BillingsReadOnlyRepositoryBuilder GetById(Billing? billing)
    {
        _repository
            .Setup(repository => repository.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(billing);

        return this;
    }

    public BillingsReadOnlyRepositoryBuilder GetSummary(BillingsSummaryDto summary)
    {
        _repository
            .Setup(repository => repository.GetSummary(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(summary);

        return this;
    }

    public BillingsReadOnlyRepositoryBuilder FilterByPeriod(IList<Billing> billings)
    {
        _repository
            .Setup(repository => repository.FilterByPeriod(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(billings);

        return this;
    }

    public IBillingsReadOnlyRepository Build() => _repository.Object;
}
