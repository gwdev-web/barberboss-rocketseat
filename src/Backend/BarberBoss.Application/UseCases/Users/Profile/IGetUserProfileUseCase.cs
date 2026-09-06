using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Users.Profile;

public interface IGetUserProfileUseCase
{
    /// <summary>Perfil do usuário autenticado.</summary>
    Task<ResponseUserJson> Execute();

    /// <summary>Perfil de um usuário específico. Só o próprio usuário ou um admin pode ver.</summary>
    Task<ResponseUserJson> Execute(Guid id);
}
