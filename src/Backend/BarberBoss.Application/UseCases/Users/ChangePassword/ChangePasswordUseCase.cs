using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Users;
using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly IUsersUpdateOnlyRepository _repository;
    private readonly IPasswordEncripter _passwordEncripter;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordUseCase(
        IUsersUpdateOnlyRepository repository,
        IPasswordEncripter passwordEncripter,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _passwordEncripter = passwordEncripter;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.Get();

        Validate(request, loggedUser.PasswordHash);

        var user = await _repository.GetById(loggedUser.Id)
            ?? throw new NotFoundException(ResourceMessagesException.USER_NOT_FOUND);

        user.PasswordHash = _passwordEncripter.Encrypt(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        _repository.Update(user);
        await _unitOfWork.Commit();
    }

    private void Validate(RequestChangePasswordJson request, string currentPasswordHash)
    {
        var result = new ChangePasswordValidator().Validate(request);

        if (result.IsValid == false)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());

        if (_passwordEncripter.Verify(request.CurrentPassword, currentPasswordHash) == false)
            throw new ErrorOnValidationException([ResourceMessagesException.CURRENT_PASSWORD_INVALID]);
    }
}
