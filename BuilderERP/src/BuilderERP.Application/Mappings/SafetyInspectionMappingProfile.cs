using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SafetyInspectionMappingProfile : Profile
{
    public SafetyInspectionMappingProfile()
    {
        CreateMap<SafetyInspection, SafetyInspectionDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateSafetyInspectionDto, SafetyInspection>();
        CreateMap<UpdateSafetyInspectionDto, SafetyInspection>();
    }
}
