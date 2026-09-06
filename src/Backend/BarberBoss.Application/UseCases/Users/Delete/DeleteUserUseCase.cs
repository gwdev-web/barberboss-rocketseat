using BarberBoss.Communication.Enums;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Users;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.Delete;

public class DeleteUserUseCase : IDeleteUserUseCase
{
    private readonly IUsersWriteOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserUseCase(
        IUsersWriteOnlyRepository repository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(Guid id)
    {
        var loggedUser = await _loggedUser.Get();

        if (loggedUser.Id != id && loggedUser.Role != UserRole.Admin)
            throw new ForbiddenException();

        var deleted = await _repository.Delete(id);

        if (deleted == false)
            throw new NotFoundException(ResourceMessagesException.USER_NOT_FOUND);

        await _unitOfWork.Commit();
    }
}
