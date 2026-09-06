using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace WebApi.Test.Users;

public class UserProfileTests : BarberBossClassFixture
{
    private const string METHOD = "api/users";

    public UserProfileTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Success_Own_Profile()
    {
        var response = await DoGet(METHOD, Factory.TokenFor(Factory.User));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        document.RootElement.GetProperty("email").GetString().Should().Be(Factory.User.Email);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await DoGet(METHOD);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Reading_Another_User()
    {
        var response = await DoGet($"{METHOD}/{Factory.OtherUser.Id}", Factory.TokenFor(Factory.User));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Error_Updating_Another_User()
    {
        var request = new { name = "Nome Novo", email = "novo@barberboss.com" };

        var response = await DoPut(
            $"{METHOD}/{Factory.OtherUser.Id}",
            request,
            Factory.TokenFor(Factory.User));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
