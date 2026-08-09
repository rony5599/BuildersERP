using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class WbsTaskMappingProfile : Profile
{
    public WbsTaskMappingProfile()
    {
        CreateMap<WbsTask, WbsTaskDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.ParentCode, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Code : null));
        CreateMap<CreateWbsTaskDto, WbsTask>();
        CreateMap<UpdateWbsTaskDto, WbsTask>();
    }
}
