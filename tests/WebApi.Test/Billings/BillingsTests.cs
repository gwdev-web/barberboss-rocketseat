using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace WebApi.Test.Billings;

public class BillingsTests : BarberBossClassFixture
{
    private const string METHOD = "api/billings";

    public BillingsTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await DoGet(METHOD);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Success_List_With_Token()
    {
        var response = await DoGet(METHOD, Factory.TokenFor(Factory.User));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        document.RootElement.GetProperty("items").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Success_Register()
    {
        var request = new
        {
            date = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            barberName = "Rafael Souza",
            clientName = "Marcos Lima",
            serviceName = "Degradê",
            amount = 60.00m,
            paymentMethod = 1,
            status = 0,
            notes = "Pagou em dinheiro",
        };

        var response = await DoPost(METHOD, request, Factory.TokenFor(Factory.User));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Error_Cancelled_Billing_With_Amount()
    {
        var request = new
        {
            date = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
            barberName = "Rafael Souza",
            clientName = "Marcos Lima",
            serviceName = "Degradê",
            amount = 60.00m,
            paymentMethod = 1,
            status = 1,
        };

        var response = await DoPost(METHOD, request, Factory.TokenFor(Factory.User));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Error_Billing_From_Another_User()
    {
        var response = await DoGet(
            $"{METHOD}/{Factory.Billing.Id}",
            Factory.TokenFor(Factory.OtherUser));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Success_Summary_Ignores_Cancelled()
    {
        var response = await DoGet($"{METHOD}/summary", Factory.TokenFor(Factory.User));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        document.RootElement.GetProperty("total").GetDecimal().Should().BeGreaterThanOrEqualTo(0);
    }
}
