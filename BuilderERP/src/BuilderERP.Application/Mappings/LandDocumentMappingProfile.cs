using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class LandDocumentMappingProfile : Profile
{
    public LandDocumentMappingProfile()
    {
        CreateMap<LandDocument, LandDocumentDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateLandDocumentDto, LandDocument>();
        CreateMap<UpdateLandDocumentDto, LandDocument>();
    }
}
