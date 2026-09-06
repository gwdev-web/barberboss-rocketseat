using BarberBoss.Communication.Enums;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Security.Tokens;
using BarberBoss.Infrastructure.DataAccess;
using BarberBoss.Infrastructure.Security.Cryptography;
using BarberBoss.Infrastructure.Security.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApi.Test;

/// <summary>
/// Sobe a API inteira em memória, trocando o MySQL pelo provider InMemory.
/// Tudo o mais (filtros, autenticação, casos de uso) roda igual à produção.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string SIGNING_KEY = "chave-de-teste-de-integracao-do-barberboss-1234567890";

    // O root explícito garante que o seed e a aplicação enxerguem o mesmo banco em memória.
    private readonly InMemoryDatabaseRoot _databaseRoot = new();
    private readonly string _databaseName = $"barberboss-tests-{Guid.NewGuid()}";

    public User User { get; private set; } = default!;
    public User OtherUser { get; private set; } = default!;
    public Billing Billing { get; private set; } = default!;
    public string Password { get; } = "barberboss123";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Connection"] = "Server=localhost;Database=barberboss_test;Uid=test;Pwd=test;",
                ["Settings:Jwt:SigningKey"] = SIGNING_KEY,
                ["Settings:Jwt:ExpirationTimeMinutes"] = "60",
                ["Settings:Jwt:Issuer"] = "BarberBoss",
                ["Settings:Jwt:Audience"] = "BarberBossClient",
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<BarberBossDbContext>));
            services.RemoveAll(typeof(DbContextOptions));

            services.AddDbContext<BarberBossDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName, _databaseRoot));

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BarberBossDbContext>();

            Seed(dbContext);
        });
    }

    public string TokenFor(User user)
    {
        var generator = new JwtTokenGenerator(new JwtSettings
        {
            SigningKey = SIGNING_KEY,
            ExpirationTimeMinutes = 60,
            Issuer = "BarberBoss",
            Audience = "BarberBossClient",
        });

        return generator.Generate(user).Token;
    }

    private void Seed(BarberBossDbContext dbContext)
    {
        var encripter = new PasswordEncripter();

        User = new User
        {
            Id = Guid.NewGuid(),
            Name = "Rafael Souza",
            Email = "rafael@barberboss.com",
            PasswordHash = encripter.Encrypt(Password),
            Role = UserRole.User,
        };

        OtherUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Carla Dias",
            Email = "carla@barberboss.com",
            PasswordHash = encripter.Encrypt(Password),
            Role = UserRole.User,
        };

        Billing = new Billing
        {
            Id = Guid.NewGuid(),
            UserId = User.Id,
            Date = DateOnly.FromDateTime(DateTime.Today),
            BarberName = "Rafael Souza",
            ClientName = "João Pedro",
            ServiceName = "Corte + Barba",
            Amount = 75m,
            PaymentMethod = PaymentMethod.Pix,
            Status = BillingStatus.Paid,
        };

        dbContext.Users.AddRange(User, OtherUser);
        dbContext.Billings.Add(Billing);
        dbContext.SaveChanges();
    }
}
