using BarberBoss.Api.Extensions;
using BarberBoss.Api.Filters;
using BarberBoss.Application;
using BarberBoss.Infrastructure;
using BarberBoss.Infrastructure.Extensions;
using QuestPDF.Infrastructure;

// Licença Community do QuestPDF (gratuita para uso open source / empresas pequenas).
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = InvalidModelStateResponse.Build;
});

builder.Services.AddSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BarberBoss API v1");
    options.RoutePrefix = "swagger";
});

// Dentro do container só expomos HTTP; o TLS fica a cargo do proxy/Azure.
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// O Kestrel escuta em todas as interfaces do container e o log dele mostra o endereço
// de bind (0.0.0.0), que não serve para abrir no navegador. O aviso abaixo imprime a
// URL que você usa de fato no host, montada pelo docker compose a partir de API_PORT.
app.Lifetime.ApplicationStarted.Register(() =>
{
    var publicUrl = app.Configuration["Settings:PublicUrl"];

    if (string.IsNullOrWhiteSpace(publicUrl))
        return;

    publicUrl = publicUrl.TrimEnd('/');

    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("BarberBoss");

    logger.LogInformation("BarberBoss API pronta em {PublicUrl}", publicUrl);
    logger.LogInformation("Swagger em {SwaggerUrl}", $"{publicUrl}/swagger");
});

await app.Services.MigrateDatabase();

app.Run();

/// <summary>Exposto para o WebApplicationFactory dos testes de integração.</summary>
public partial class Program { }
