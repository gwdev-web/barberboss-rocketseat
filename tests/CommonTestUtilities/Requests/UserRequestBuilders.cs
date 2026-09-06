using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestUtilities.Requests;

public static class RequestRegisterUserJsonBuilder
{
    public static RequestRegisterUserJson Build(int passwordLength = 10)
        => new Faker<RequestRegisterUserJson>("pt_BR")
            .RuleFor(request => request.Name, faker => faker.Name.FullName())
            .RuleFor(request => request.Email, faker => faker.Internet.Email().ToLowerInvariant())
            .RuleFor(request => request.Password, faker => faker.Internet.Password(passwordLength))
            .Generate();
}

public static class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
        => new Faker<RequestUpdateUserJson>("pt_BR")
            .RuleFor(request => request.Name, faker => faker.Name.FullName())
            .RuleFor(request => request.Email, faker => faker.Internet.Email().ToLowerInvariant())
            .Generate();
}

public static class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build(string? email = null, string? password = null)
        => new()
        {
            Email = email ?? new Faker().Internet.Email().ToLowerInvariant(),
            Password = password ?? "barberboss123",
        };
}

public static class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(
        string currentPassword = "barberboss123",
        string newPassword = "novaSenha456")
        => new() { CurrentPassword = currentPassword, NewPassword = newPassword };
}
