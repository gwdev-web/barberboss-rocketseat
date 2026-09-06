using BarberBoss.Communication.Enums;
using BarberBoss.Domain.Entities;
using Bogus;
using CommonTestUtilities.Cryptography;

namespace CommonTestUtilities.Entities;

public static class UserBuilder
{
    public const string DEFAULT_PASSWORD = "barberboss123";

    public static User Build(UserRole role = UserRole.User, string password = DEFAULT_PASSWORD)
    {
        var encripter = PasswordEncripterBuilder.Build();

        return new Faker<User>("pt_BR")
            .RuleFor(user => user.Id, _ => Guid.NewGuid())
            .RuleFor(user => user.Name, faker => faker.Name.FullName())
            .RuleFor(user => user.Email, faker => faker.Internet.Email().ToLowerInvariant())
            .RuleFor(user => user.PasswordHash, _ => encripter.Encrypt(password))
            .RuleFor(user => user.Role, _ => role)
            .RuleFor(user => user.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(user => user.UpdatedAt, _ => DateTime.UtcNow)
            .Generate();
    }
}
