using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ServiceTicketMappingProfile : Profile
{
    public ServiceTicketMappingProfile()
    {
        CreateMap<ServiceTicket, ServiceTicketDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateServiceTicketDto, ServiceTicket>();
        CreateMap<UpdateServiceTicketDto, ServiceTicket>();
    }
}
