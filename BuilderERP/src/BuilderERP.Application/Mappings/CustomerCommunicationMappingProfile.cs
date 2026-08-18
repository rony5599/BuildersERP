using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CustomerCommunicationMappingProfile : Profile
{
    public CustomerCommunicationMappingProfile()
    {
        CreateMap<CustomerCommunication, CustomerCommunicationDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty));
        CreateMap<CreateCustomerCommunicationDto, CustomerCommunication>();
        CreateMap<UpdateCustomerCommunicationDto, CustomerCommunication>();
    }
}
