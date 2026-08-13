using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class LegalCaseMappingProfile : Profile
{
    public LegalCaseMappingProfile()
    {
        CreateMap<LegalCase, LegalCaseDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateLegalCaseDto, LegalCase>();
        CreateMap<UpdateLegalCaseDto, LegalCase>();
    }
}
