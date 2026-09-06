using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestUtilities.Requests;

public static class RequestBillingJsonBuilder
{
    public static RequestBillingJson Build(BillingStatus status = BillingStatus.Paid)
    {
        return new Faker<RequestBillingJson>("pt_BR")
            .RuleFor(request => request.Date, faker => DateOnly.FromDateTime(faker.Date.Recent(20)))
            .RuleFor(request => request.BarberName, faker => faker.Name.FullName())
            .RuleFor(request => request.ClientName, faker => faker.Name.FullName())
            .RuleFor(request => request.ServiceName, faker => faker.PickRandom("Corte", "Barba", "Combo", "Platinado"))
            .RuleFor(request => request.PaymentMethod, faker => faker.PickRandom<PaymentMethod>())
            .RuleFor(request => request.Status, _ => status)
            .RuleFor(request => request.Amount, faker => status == BillingStatus.Cancelled
                ? 0m
                : Math.Round(faker.Random.Decimal(20, 250), 2))
            .RuleFor(request => request.Notes, faker => faker.Lorem.Sentence(6))
            .Generate();
    }
}
