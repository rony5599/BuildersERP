using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class LegalAgreementMappingProfile : Profile
{
    public LegalAgreementMappingProfile()
    {
        CreateMap<LegalAgreement, LegalAgreementDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateLegalAgreementDto, LegalAgreement>();
        CreateMap<UpdateLegalAgreementDto, LegalAgreement>();
    }
}
