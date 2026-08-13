using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class LandRegistrationMappingProfile : Profile
{
    public LandRegistrationMappingProfile()
    {
        CreateMap<LandRegistration, LandRegistrationDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateLandRegistrationDto, LandRegistration>();
        CreateMap<UpdateLandRegistrationDto, LandRegistration>();
    }
}
