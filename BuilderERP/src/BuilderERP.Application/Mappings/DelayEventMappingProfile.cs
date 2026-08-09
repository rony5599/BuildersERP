using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DelayEventMappingProfile : Profile
{
    public DelayEventMappingProfile()
    {
        CreateMap<DelayEvent, DelayEventDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.WbsTaskCode, opt => opt.MapFrom(src => src.WbsTask != null ? src.WbsTask.Code : null));
        CreateMap<CreateDelayEventDto, DelayEvent>();
        CreateMap<UpdateDelayEventDto, DelayEvent>();
    }
}
