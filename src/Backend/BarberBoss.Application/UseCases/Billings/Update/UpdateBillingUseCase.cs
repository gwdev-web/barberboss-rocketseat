using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Update;

public class UpdateBillingUseCase : IUpdateBillingUseCase
{
    private readonly IBillingsUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateBillingUseCase(
        IBillingsUpdateOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Execute(Guid id, RequestBillingJson request)
    {
        Validate(request);

        var billing = await _repository.GetById(id)
            ?? throw new NotFoundException(ResourceMessagesException.BILLING_NOT_FOUND);

        _mapper.Map(request, billing);
        billing.UpdatedAt = DateTime.UtcNow;

        _repository.Update(billing);
        await _unitOfWork.Commit();
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
