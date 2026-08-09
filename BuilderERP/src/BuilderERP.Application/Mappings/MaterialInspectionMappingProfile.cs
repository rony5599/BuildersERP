using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class MaterialInspectionMappingProfile : Profile
{
    public MaterialInspectionMappingProfile()
    {
        CreateMap<MaterialInspection, MaterialInspectionDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateMaterialInspectionDto, MaterialInspection>();
        CreateMap<UpdateMaterialInspectionDto, MaterialInspection>();
    }
}
