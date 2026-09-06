using AutoMapper;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.GetById;

public class GetBillingByIdUseCase : IGetBillingByIdUseCase
{
    private readonly IBillingsReadOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;

    public GetBillingByIdUseCase(
        IBillingsReadOnlyRepository repository,
        ILoggedUser loggedUser,
        IMapper mapper)
    {
        _repository = repository;
        _loggedUser = loggedUser;
        _mapper = mapper;
    }

    public async Task<ResponseBillingJson> Execute(Guid id)
    {
        var loggedUser = await _loggedUser.Get();

        var billing = await _repository.GetById(loggedUser.Id, id)
            ?? throw new NotFoundException(ResourceMessagesException.BILLING_NOT_FOUND);

        return _mapper.Map<ResponseBillingJson>(billing);
    }
}
