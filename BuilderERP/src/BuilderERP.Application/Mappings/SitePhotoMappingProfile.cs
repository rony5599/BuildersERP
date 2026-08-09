using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SitePhotoMappingProfile : Profile
{
    public SitePhotoMappingProfile()
    {
        CreateMap<SitePhoto, SitePhotoDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.DailyProgressDate, opt => opt.MapFrom(src => src.DailyProgress != null ? (DateTime?)src.DailyProgress.ProgressDate : null));
        CreateMap<CreateSitePhotoDto, SitePhoto>();
        CreateMap<UpdateSitePhotoDto, SitePhoto>();
    }
}
