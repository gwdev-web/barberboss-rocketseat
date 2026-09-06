using BarberBoss.Application.UseCases.Users;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Users;
using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Domain.Security.Tokens;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Login.DoLogin;

public class DoLoginUseCase : IDoLoginUseCase
{
    private readonly IUsersReadOnlyRepository _repository;
    private readonly IPasswordEncripter _passwordEncripter;
    private readonly IAccessTokenGenerator _tokenGenerator;

    public DoLoginUseCase(
        IUsersReadOnlyRepository repository,
        IPasswordEncripter passwordEncripter,
        IAccessTokenGenerator tokenGenerator)
    {
        _repository = repository;
        _passwordEncripter = passwordEncripter;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ResponseLoginJson> Execute(RequestLoginJson request)
    {
        var result = new LoginValidator().Validate(request);

        if (result.IsValid == false)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());

        var user = await _repository.GetByEmail(request.Email.Trim().ToLowerInvariant())
            ?? throw new InvalidLoginException();

        // Mensagem genérica de propósito: não revela se o e-mail existe.
        if (_passwordEncripter.Verify(request.Password, user.PasswordHash) == false)
            throw new InvalidLoginException();

        var accessToken = _tokenGenerator.Generate(user);

        return new ResponseLoginJson
        {
            Id = user.Id,
            Name = user.Name,
            Token = accessToken.Token,
            ExpiresAt = accessToken.ExpiresAt,
        };
    }
}
