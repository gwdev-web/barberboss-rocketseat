using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Dtos;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.GetAll;

public class GetAllBillingsUseCase : IGetAllBillingsUseCase
{
    private readonly IBillingsReadOnlyRepository _repository;
    private readonly IMapper _mapper;

    public GetAllBillingsUseCase(IBillingsReadOnlyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponsePagedJson<ResponseShortBillingJson>> Execute(RequestBillingFilterJson request)
    {
        Validate(request);

        var filter = _mapper.Map<FilterBillingsDto>(request);
        var result = await _repository.GetAll(filter);

        var totalPages = filter.PageSize == 0
            ? 0
            : (int)Math.Ceiling(result.TotalItems / (double)filter.PageSize);

        return new ResponsePagedJson<ResponseShortBillingJson>
        {
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = totalPages,
            Items = _mapper.Map<IList<ResponseShortBillingJson>>(result.Items),
        };
    }

    private static void Validate(RequestBillingFilterJson request)
    {
        var result = new BillingFilterValidator().Validate(request);

        if (result.IsValid == false)
        {
            var errors = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }
    }
}
