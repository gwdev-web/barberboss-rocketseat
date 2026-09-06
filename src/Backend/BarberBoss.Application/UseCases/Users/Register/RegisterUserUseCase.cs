using AutoMapper;
using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Users;
using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Domain.Security.Tokens;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUsersWriteOnlyRepository _writeOnlyRepository;
    private readonly IUsersReadOnlyRepository _readOnlyRepository;
    private readonly IPasswordEncripter _passwordEncripter;
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterUserUseCase(
        IUsersWriteOnlyRepository writeOnlyRepository,
        IUsersReadOnlyRepository readOnlyRepository,
        IPasswordEncripter passwordEncripter,
        IAccessTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _passwordEncripter = passwordEncripter;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        await Validate(request);

        var user = _mapper.Map<User>(request);
        user.Id = Guid.NewGuid();
        user.Email = request.Email.Trim().ToLowerInvariant();
        user.PasswordHash = _passwordEncripter.Encrypt(request.Password);
        user.Role = UserRole.User;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _writeOnlyRepository.Add(user);
        await _unitOfWork.Commit();

        var response = _mapper.Map<ResponseRegisteredUserJson>(user);
        response.Token = _tokenGenerator.Generate(user).Token;

        return response;
    }

    private async Task Validate(RequestRegisterUserJson request)
    {
        var result = new RegisterUserValidator().Validate(request);

        if (result.IsValid == false)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());

        // E-mail é único: devolve 409 em vez de estourar erro de constraint no banco.
        var emailAlreadyRegistered = await _readOnlyRepository
            .ExistsUserWithEmail(request.Email.Trim().ToLowerInvariant());

        if (emailAlreadyRegistered)
            throw new ConflictException(ResourceMessagesException.EMAIL_ALREADY_REGISTERED);
    }
}
