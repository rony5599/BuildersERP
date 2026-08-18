using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class TowerMappingProfile : Profile
{
    public TowerMappingProfile()
    {
        CreateMap<Tower, TowerDto>()
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Building != null ? src.Building.ProjectId : Guid.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Building != null && src.Building.Project != null ? src.Building.Project.Name : string.Empty));
        CreateMap<CreateTowerDto, Tower>();
        CreateMap<UpdateTowerDto, Tower>();
    }
}
