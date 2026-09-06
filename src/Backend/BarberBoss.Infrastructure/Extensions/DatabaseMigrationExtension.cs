using BarberBoss.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Infrastructure.Extensions;

public static class DatabaseMigrationExtension
{
    /// <summary>
    /// Aplica as migrations pendentes na inicialização. Dentro do Docker o container
    /// da API pode subir antes do MySQL aceitar conexões, por isso o retry.
    /// </summary>
    public static async Task MigrateDatabase(this IServiceProvider serviceProvider, int attempts = 10)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BarberBossDbContext>();

        // Os testes de integração usam o provider InMemory, que não tem migrations.
        if (dbContext.Database.IsRelational() == false)
            return;

        for (var attempt = 1; attempt <= attempts; attempt++)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                return;
            }
            catch (System.Exception) when (attempt < attempts)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
