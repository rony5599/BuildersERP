using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class BrokerMappingProfile : Profile
{
    public BrokerMappingProfile()
    {
        CreateMap<Broker, BrokerDto>();
        CreateMap<CreateBrokerDto, Broker>();
        CreateMap<UpdateBrokerDto, Broker>();
    }
}
