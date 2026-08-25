using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PropertyUnitMappingProfile : Profile
{
    public PropertyUnitMappingProfile()
    {
        CreateMap<PropertyUnit, PropertyUnitDto>()
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Floor != null && src.Floor.Tower != null && src.Floor.Tower.Building != null ? src.Floor.Tower.Building.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Floor != null && src.Floor.Tower != null && src.Floor.Tower.Building != null && src.Floor.Tower.Building.Project != null ? src.Floor.Tower.Building.Project.Name : string.Empty));
        CreateMap<CreatePropertyUnitDto, PropertyUnit>();
        CreateMap<UpdatePropertyUnitDto, PropertyUnit>();
    }
}
