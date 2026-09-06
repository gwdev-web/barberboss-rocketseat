using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace WebApi.Test.HealthChecks;

public class HealthCheckTests : BarberBossClassFixture
{
    public HealthCheckTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Liveness_Is_Public_And_Healthy()
    {
        var response = await DoGet("health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        document.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Fact]
    public async Task Readiness_Reports_The_Database_Check()
    {
        var response = await DoGet("health/ready");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        document.RootElement
            .GetProperty("checks")
            .EnumerateArray()
            .Should()
            .Contain(check => check.GetProperty("name").GetString() == "database");
    }
}
