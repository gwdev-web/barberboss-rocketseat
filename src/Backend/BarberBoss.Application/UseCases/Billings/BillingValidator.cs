using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;

/// <summary>
/// Regras de negócio compartilhadas entre o cadastro e a atualização de um faturamento.
/// </summary>
public class BillingValidator : AbstractValidator<RequestBillingJson>
{
    public BillingValidator()
    {
        RuleFor(billing => billing.Date)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.DATE_REQUIRED)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage(ResourceMessagesException.DATE_CANNOT_BE_IN_THE_FUTURE);

        RuleFor(billing => billing.BarberName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.BARBER_NAME_REQUIRED)
            .Length(2, 80).WithMessage(ResourceMessagesException.BARBER_NAME_LENGTH);

        RuleFor(billing => billing.ClientName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.CLIENT_NAME_REQUIRED)
            .Length(2, 120).WithMessage(ResourceMessagesException.CLIENT_NAME_LENGTH);

        RuleFor(billing => billing.ServiceName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.SERVICE_NAME_REQUIRED)
            .Length(2, 120).WithMessage(ResourceMessagesException.SERVICE_NAME_LENGTH);

        RuleFor(billing => billing.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ResourceMessagesException.AMOUNT_MUST_BE_GREATER_OR_EQUAL_ZERO);

        RuleFor(billing => billing.PaymentMethod)
            .IsInEnum().WithMessage(ResourceMessagesException.PAYMENT_METHOD_NOT_SUPPORTED);

        RuleFor(billing => billing.Status)
            .IsInEnum().WithMessage(ResourceMessagesException.STATUS_NOT_SUPPORTED);

        RuleFor(billing => billing.Notes)
            .MaximumLength(500)
            .WithMessage(ResourceMessagesException.NOTES_MAX_LENGTH);

        // Regra do desafio: cancelado tem que ter valor zerado.
        RuleFor(billing => billing.Amount)
            .Equal(0)
            .WithMessage(ResourceMessagesException.CANCELLED_BILLING_MUST_HAVE_ZERO_AMOUNT)
            .When(billing => billing.Status == BillingStatus.Cancelled);
    }
}
