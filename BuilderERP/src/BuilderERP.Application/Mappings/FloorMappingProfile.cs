using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class FloorMappingProfile : Profile
{
    public FloorMappingProfile()
    {
        CreateMap<Floor, FloorDto>()
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Tower != null && src.Tower.Building != null ? src.Tower.Building.ProjectId : Guid.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Tower != null && src.Tower.Building != null && src.Tower.Building.Project != null ? src.Tower.Building.Project.Name : string.Empty));
        CreateMap<CreateFloorDto, Floor>();
        CreateMap<UpdateFloorDto, Floor>();
    }
}
