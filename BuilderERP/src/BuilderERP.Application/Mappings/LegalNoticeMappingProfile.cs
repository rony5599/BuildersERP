using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class LegalNoticeMappingProfile : Profile
{
    public LegalNoticeMappingProfile()
    {
        CreateMap<LegalNotice, LegalNoticeDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateLegalNoticeDto, LegalNotice>();
        CreateMap<UpdateLegalNoticeDto, LegalNotice>();
    }
}
