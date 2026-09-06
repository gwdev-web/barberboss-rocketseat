using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Dtos;
using BarberBoss.Domain.Entities;

namespace BarberBoss.Application.AutoMapper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        RequestToDomain();
        DomainToResponse();
    }

    private void RequestToDomain()
    {
        CreateMap<RequestBillingJson, Billing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<RequestBillingFilterJson, FilterBillingsDto>();
    }

    private void DomainToResponse()
    {
        CreateMap<Billing, ResponseBillingJson>();
        CreateMap<Billing, ResponseShortBillingJson>();
        CreateMap<Billing, ResponseRegisteredBillingJson>();
    }
}
