using BarberBoss.Application.UseCases.Billings;
using BarberBoss.Communication.Enums;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Billings;

public class BillingValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_BarberName_Empty(string? barberName)
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.BarberName = barberName!;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.BARBER_NAME_REQUIRED));
    }

    [Fact]
    public void Error_BarberName_Too_Long()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.BarberName = new string('a', 81);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.BARBER_NAME_LENGTH));
    }

    [Fact]
    public void Error_ClientName_Empty()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.ClientName = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.CLIENT_NAME_REQUIRED));
    }

    [Fact]
    public void Error_ServiceName_Empty()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.ServiceName = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.SERVICE_NAME_REQUIRED));
    }

    [Fact]
    public void Error_Amount_Negative()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.Amount = -1;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.AMOUNT_MUST_BE_GREATER_OR_EQUAL_ZERO));
    }

    [Fact]
    public void Error_Cancelled_Billing_With_Amount()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build(BillingStatus.Cancelled);
        request.Amount = 50;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error =>
                error.ErrorMessage.Equals(ResourceMessagesException.CANCELLED_BILLING_MUST_HAVE_ZERO_AMOUNT));
    }

    [Fact]
    public void Success_Cancelled_Billing_With_Zero_Amount()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build(BillingStatus.Cancelled);

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Error_Date_In_The_Future()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error =>
                error.ErrorMessage.Equals(ResourceMessagesException.DATE_CANNOT_BE_IN_THE_FUTURE));
    }

    [Fact]
    public void Error_PaymentMethod_Invalid()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.PaymentMethod = (PaymentMethod)999;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error =>
                error.ErrorMessage.Equals(ResourceMessagesException.PAYMENT_METHOD_NOT_SUPPORTED));
    }

    [Fact]
    public void Error_Notes_Too_Long()
    {
        var validator = new BillingValidator();
        var request = RequestBillingJsonBuilder.Build();
        request.Notes = new string('a', 501);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesException.NOTES_MAX_LENGTH));
    }
}
