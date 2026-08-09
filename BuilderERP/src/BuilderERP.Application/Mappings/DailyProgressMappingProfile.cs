using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DailyProgressMappingProfile : Profile
{
    public DailyProgressMappingProfile()
    {
        CreateMap<DailyProgress, DailyProgressDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateDailyProgressDto, DailyProgress>();
        CreateMap<UpdateDailyProgressDto, DailyProgress>();
    }
}
