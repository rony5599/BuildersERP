using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SiteInspectionMappingProfile : Profile
{
    public SiteInspectionMappingProfile()
    {
        CreateMap<SiteInspection, SiteInspectionDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateSiteInspectionDto, SiteInspection>();
        CreateMap<UpdateSiteInspectionDto, SiteInspection>();
    }
}
