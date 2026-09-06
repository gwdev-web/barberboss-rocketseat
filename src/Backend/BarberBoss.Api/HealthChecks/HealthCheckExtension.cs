using System.Text.Json;
using BarberBoss.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BarberBoss.Api.HealthChecks;

public static class HealthCheckExtension
{
    private const string READY_TAG = "ready";

    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<BarberBossDbContext>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: [READY_TAG]);

        return services;
    }

    public static WebApplication MapApiHealthChecks(this WebApplication app)
    {
        // Liveness: só responde se o processo está de pé. Não toca no banco.
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = WriteResponse,
        });

        // Readiness: valida também a conexão com o MySQL. É o que o Azure deve monitorar.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(READY_TAG),
            ResponseWriter = WriteResponse,
        });

        return app;
    }

    private static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMs = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
            }),
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
