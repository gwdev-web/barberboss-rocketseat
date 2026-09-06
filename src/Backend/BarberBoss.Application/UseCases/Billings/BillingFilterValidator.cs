using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;

public class BillingFilterValidator : AbstractValidator<RequestBillingFilterJson>
{
    public BillingFilterValidator()
    {
        RuleFor(filter => filter.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(ResourceMessagesException.PAGE_NUMBER_INVALID);

        RuleFor(filter => filter.PageSize)
            .InclusiveBetween(1, 100).WithMessage(ResourceMessagesException.PAGE_SIZE_INVALID);

        RuleFor(filter => filter.StartDate)
            .LessThanOrEqualTo(filter => filter.EndDate)
            .When(filter => filter.StartDate.HasValue && filter.EndDate.HasValue)
            .WithMessage(ResourceMessagesException.INVALID_PERIOD);
    }
}
