using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Users;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserValidator()
    {
        RuleFor(user => user.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_REQUIRED)
            .Length(2, 100).WithMessage(ResourceMessagesException.NAME_LENGTH);

        RuleFor(user => user.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.EMAIL_REQUIRED)
            .EmailAddress().WithMessage(ResourceMessagesException.EMAIL_INVALID);

        RuleFor(user => user.Password).SetValidator(new PasswordValidator());
    }
}

public class UpdateUserValidator : AbstractValidator<RequestUpdateUserJson>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_REQUIRED)
            .Length(2, 100).WithMessage(ResourceMessagesException.NAME_LENGTH);

        RuleFor(user => user.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.EMAIL_REQUIRED)
            .EmailAddress().WithMessage(ResourceMessagesException.EMAIL_INVALID);
    }
}

public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
        => RuleFor(request => request.NewPassword).SetValidator(new PasswordValidator());
}

public class LoginValidator : AbstractValidator<RequestLoginJson>
{
    public LoginValidator()
    {
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.EMAIL_REQUIRED)
            .EmailAddress().WithMessage(ResourceMessagesException.EMAIL_INVALID);

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage(ResourceMessagesException.PASSWORD_REQUIRED);
    }
}

/// <summary>Política de senha compartilhada entre cadastro e troca de senha.</summary>
public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.PASSWORD_REQUIRED)
            .MinimumLength(6).WithMessage(ResourceMessagesException.PASSWORD_MIN_LENGTH);
    }
}
