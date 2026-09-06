using AutoMapper;
using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Users;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.Profile;

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly IUsersReadOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;

    public GetUserProfileUseCase(
        IUsersReadOnlyRepository repository,
        ILoggedUser loggedUser,
        IMapper mapper)
    {
        _repository = repository;
        _loggedUser = loggedUser;
        _mapper = mapper;
    }

    public async Task<ResponseUserJson> Execute()
    {
        var user = await _loggedUser.Get();

        return _mapper.Map<ResponseUserJson>(user);
    }

    public async Task<ResponseUserJson> Execute(Guid id)
    {
        var loggedUser = await _loggedUser.Get();

        if (loggedUser.Id != id && loggedUser.Role != UserRole.Admin)
            throw new ForbiddenException();

        var user = await _repository.GetById(id)
            ?? throw new NotFoundException(ResourceMessagesException.USER_NOT_FOUND);

        return _mapper.Map<ResponseUserJson>(user);
    }
}
