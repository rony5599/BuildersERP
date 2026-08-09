using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DrawingMappingProfile : Profile
{
    public DrawingMappingProfile()
    {
        CreateMap<Drawing, DrawingDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateDrawingDto, Drawing>();
        CreateMap<UpdateDrawingDto, Drawing>();
    }
}
