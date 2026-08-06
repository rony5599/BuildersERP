using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PropertyUnitMappingProfile : Profile
{
    public PropertyUnitMappingProfile()
    {
        CreateMap<PropertyUnit, PropertyUnitDto>();
        CreateMap<CreatePropertyUnitDto, PropertyUnit>();
        CreateMap<UpdatePropertyUnitDto, PropertyUnit>();
    }
}
