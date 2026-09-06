using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace WebApi.Test.Login;

public class DoLoginTests : BarberBossClassFixture
{
    private const string METHOD = "api/login";

    public DoLoginTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Success()
    {
        var request = new { email = Factory.User.Email, password = Factory.Password };

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStreamAsync();
        var document = await JsonDocument.ParseAsync(body);

        document.RootElement.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
        document.RootElement.GetProperty("name").GetString().Should().Be(Factory.User.Name);
    }

    [Fact]
    public async Task Error_Wrong_Password()
    {
        var request = new { email = Factory.User.Email, password = "senha-errada" };

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Unknown_Email()
    {
        var request = new { email = "ninguem@barberboss.com", password = Factory.Password };

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Invalid_Email_Format()
    {
        var request = new { email = "sem-arroba", password = Factory.Password };

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
