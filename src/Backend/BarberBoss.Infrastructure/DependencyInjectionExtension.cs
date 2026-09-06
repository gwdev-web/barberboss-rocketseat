using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Repositories.Users;
using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Domain.Security.Tokens;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Infrastructure.DataAccess;
using BarberBoss.Infrastructure.DataAccess.Repositories;
using BarberBoss.Infrastructure.Security.Cryptography;
using BarberBoss.Infrastructure.Security.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Infrastructure;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddRepositories(services);
        AddSecurity(services, configuration);

        services.AddHttpContextAccessor();
        services.AddScoped<ILoggedUser, Services.LoggedUser.LoggedUser>();

        return services;
    }

    public static JwtSettings ReadJwtSettings(IConfiguration configuration)
    {
        var settings = configuration.GetSection(JwtSettings.SECTION).Get<JwtSettings>() ?? new JwtSettings();

        if (string.IsNullOrWhiteSpace(settings.SigningKey) || settings.SigningKey.Length < 32)
        {
            throw new InvalidOperationException(
                "Settings:Jwt:SigningKey precisa estar configurada com pelo menos 32 caracteres.");
        }

        return settings;
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection")
            ?? throw new InvalidOperationException("A connection string 'Connection' não foi configurada.");

        // Versão fixa em vez de ServerVersion.AutoDetect: o AutoDetect abre uma conexão
        // durante o startup e quebra quando o container do MySQL ainda não está pronto.
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

        services.AddDbContext<BarberBossDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        });
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IBillingsWriteOnlyRepository, BillingsRepository>();
        services.AddScoped<IBillingsReadOnlyRepository, BillingsRepository>();
        services.AddScoped<IBillingsUpdateOnlyRepository, BillingsRepository>();

        services.AddScoped<IUsersWriteOnlyRepository, UsersRepository>();
        services.AddScoped<IUsersReadOnlyRepository, UsersRepository>();
        services.AddScoped<IUsersUpdateOnlyRepository, UsersRepository>();
    }

    private static void AddSecurity(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = ReadJwtSettings(configuration);

        services.AddSingleton(jwtSettings);
        services.AddScoped<IPasswordEncripter, PasswordEncripter>();
        services.AddScoped<IAccessTokenGenerator, JwtTokenGenerator>();
    }
}
