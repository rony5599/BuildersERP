using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class VisitorLogMappingProfile : Profile
{
    public VisitorLogMappingProfile()
    {
        CreateMap<VisitorLog, VisitorLogDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateVisitorLogDto, VisitorLog>();
        CreateMap<UpdateVisitorLogDto, VisitorLog>();
    }
}
