using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace WebApi.Test.Users;

public class RegisterUserTests : BarberBossClassFixture
{
    private const string METHOD = "api/users";

    public RegisterUserTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        document.RootElement.GetProperty("name").GetString().Should().Be(request.Name);
        document.RootElement.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = Factory.User.Email;

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Error_Short_Password()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = "123";

        var response = await DoPost(METHOD, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
