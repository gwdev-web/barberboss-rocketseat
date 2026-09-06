using BarberBoss.Application.UseCases.Billings;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Billings;

public class BillingFilterValidatorTests
{
    [Fact]
    public void Success()
    {
        var result = new BillingFilterValidator().Validate(RequestBillingFilterJsonBuilder.Build());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Error_Invalid_PageNumber(int pageNumber)
    {
        var request = RequestBillingFilterJsonBuilder.Build(pageNumber: pageNumber);

        var result = new BillingFilterValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.PAGE_NUMBER_INVALID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Error_Invalid_PageSize(int pageSize)
    {
        var request = RequestBillingFilterJsonBuilder.Build(pageSize: pageSize);

        var result = new BillingFilterValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.PAGE_SIZE_INVALID));
    }

    [Fact]
    public void Error_StartDate_Greater_Than_EndDate()
    {
        var request = RequestBillingFilterJsonBuilder.Build();
        request.StartDate = new DateOnly(2025, 3, 10);
        request.EndDate = new DateOnly(2025, 3, 1);

        var result = new BillingFilterValidator().Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.INVALID_PERIOD));
    }
}
