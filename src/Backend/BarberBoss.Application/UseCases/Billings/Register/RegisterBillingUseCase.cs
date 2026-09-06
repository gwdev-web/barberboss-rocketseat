using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase : IRegisterBillingUseCase
{
    private readonly IBillingsWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterBillingUseCase(
        IBillingsWriteOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseRegisteredBillingJson> Execute(RequestBillingJson request)
    {
        Validate(request);

        var billing = _mapper.Map<Billing>(request);
        billing.Id = Guid.NewGuid();
        billing.CreatedAt = DateTime.UtcNow;
        billing.UpdatedAt = DateTime.UtcNow;

        await _repository.Add(billing);
        await _unitOfWork.Commit();

        return _mapper.Map<ResponseRegisteredBillingJson>(billing);
    }

    private static void Validate(RequestBillingJson request)
    {
        var result = new BillingValidator().Validate(request);

        if (result.IsValid == false)
        {
            var errors = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }
    }
}
