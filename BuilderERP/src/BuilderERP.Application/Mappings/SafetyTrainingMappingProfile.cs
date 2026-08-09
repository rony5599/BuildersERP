using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SafetyTrainingMappingProfile : Profile
{
    public SafetyTrainingMappingProfile()
    {
        CreateMap<SafetyTraining, SafetyTrainingDto>()
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.Name : string.Empty));
        CreateMap<CreateSafetyTrainingDto, SafetyTraining>();
        CreateMap<UpdateSafetyTrainingDto, SafetyTraining>();
    }
}
