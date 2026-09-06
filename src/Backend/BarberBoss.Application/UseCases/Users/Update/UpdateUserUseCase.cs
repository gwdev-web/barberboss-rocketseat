using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Users;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUsersUpdateOnlyRepository _updateRepository;
    private readonly IUsersReadOnlyRepository _readRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUseCase(
        IUsersUpdateOnlyRepository updateRepository,
        IUsersReadOnlyRepository readRepository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _updateRepository = updateRepository;
        _readRepository = readRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestUpdateUserJson request)
    {
        var loggedUser = await _loggedUser.Get();

        await Execute(loggedUser.Id, request);
    }

    public async Task Execute(Guid id, RequestUpdateUserJson request)
    {
        var loggedUser = await _loggedUser.Get();

        // Autorização: só o próprio usuário (ou um admin) altera os dados.
        if (loggedUser.Id != id && loggedUser.Role != UserRole.Admin)
            throw new ForbiddenException();

        Validate(request);

        var user = await _updateRepository.GetById(id)
            ?? throw new NotFoundException(ResourceMessagesException.USER_NOT_FOUND);

        var email = request.Email.Trim().ToLowerInvariant();

        if (email != user.Email && await _readRepository.ExistsUserWithEmail(email))
            throw new ConflictException(ResourceMessagesException.EMAIL_ALREADY_REGISTERED);

        user.Name = request.Name;
        user.Email = email;
        user.UpdatedAt = DateTime.UtcNow;

        _updateRepository.Update(user);
        await _unitOfWork.Commit();
    }

    private static void Validate(RequestUpdateUserJson request)
    {
        var result = new UpdateUserValidator().Validate(request);

        if (result.IsValid == false)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
    }
}
