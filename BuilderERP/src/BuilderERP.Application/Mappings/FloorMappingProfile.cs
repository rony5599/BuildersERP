using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class FloorMappingProfile : Profile
{
    public FloorMappingProfile()
    {
        CreateMap<Floor, FloorDto>();
        CreateMap<CreateFloorDto, Floor>();
        CreateMap<UpdateFloorDto, Floor>();
    }
}
