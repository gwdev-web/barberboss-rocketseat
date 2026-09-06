using BarberBoss.Api.Extensions;
using BarberBoss.Api.Filters;
using BarberBoss.Api.HealthChecks;
using BarberBoss.Application;
using BarberBoss.Infrastructure;
using BarberBoss.Infrastructure.Extensions;
using QuestPDF.Infrastructure;

// Licença Community do QuestPDF (gratuita para uso open source / empresas pequenas).
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Em produção a connection string e a chave do JWT chegam por variável de ambiente
// (App Settings do Azure), nunca pelo appsettings versionado.
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = InvalidModelStateResponse.Build;
});

builder.Services.AddSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApiHealthChecks();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue("Settings:Swagger:Enabled", defaultValue: true))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BarberBoss API v1");
        options.RoutePrefix = "swagger";
    });
}

// Dentro do container só expomos HTTP; o TLS fica a cargo do proxy/App Service.
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapApiHealthChecks();

await app.Services.MigrateDatabase();

app.Run();

/// <summary>Exposto para o WebApplicationFactory dos testes de integração.</summary>
public partial class Program { }
