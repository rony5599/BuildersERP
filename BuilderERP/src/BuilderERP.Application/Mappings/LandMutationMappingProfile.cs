using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class LandMutationMappingProfile : Profile
{
    public LandMutationMappingProfile()
    {
        CreateMap<LandMutation, LandMutationDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateLandMutationDto, LandMutation>();
        CreateMap<UpdateLandMutationDto, LandMutation>();
    }
}
