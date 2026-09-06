using BarberBoss.Communication.Enums;
using BarberBoss.Domain.Entities;
using Bogus;

namespace CommonTestUtilities.Entities;

public static class BillingBuilder
{
    public static Billing Build(
        BillingStatus status = BillingStatus.Paid,
        DateOnly? date = null,
        Guid? userId = null)
    {
        return new Faker<Billing>("pt_BR")
            .RuleFor(billing => billing.Id, _ => Guid.NewGuid())
            .RuleFor(billing => billing.UserId, _ => userId ?? Guid.NewGuid())
            .RuleFor(billing => billing.Date, faker => date ?? DateOnly.FromDateTime(faker.Date.Recent(20)))
            .RuleFor(billing => billing.BarberName, faker => faker.Name.FullName())
            .RuleFor(billing => billing.ClientName, faker => faker.Name.FullName())
            .RuleFor(billing => billing.ServiceName, faker => faker.PickRandom("Corte", "Barba", "Combo"))
            .RuleFor(billing => billing.PaymentMethod, faker => faker.PickRandom<PaymentMethod>())
            .RuleFor(billing => billing.Status, _ => status)
            .RuleFor(billing => billing.Amount, faker => status == BillingStatus.Cancelled
                ? 0m
                : Math.Round(faker.Random.Decimal(20, 250), 2))
            .RuleFor(billing => billing.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(billing => billing.UpdatedAt, _ => DateTime.UtcNow)
            .Generate();
    }

    public static IList<Billing> Collection(int count = 5, Guid? userId = null)
        => Enumerable.Range(0, count).Select(_ => Build(userId: userId)).ToList();
}
