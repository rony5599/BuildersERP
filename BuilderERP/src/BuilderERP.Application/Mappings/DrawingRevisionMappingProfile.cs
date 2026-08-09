using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DrawingRevisionMappingProfile : Profile
{
    public DrawingRevisionMappingProfile()
    {
        CreateMap<DrawingRevision, DrawingRevisionDto>()
            .ForMember(dest => dest.DrawingNumber, opt => opt.MapFrom(src => src.Drawing != null ? src.Drawing.DrawingNumber : string.Empty));
        CreateMap<CreateDrawingRevisionDto, DrawingRevision>();
        CreateMap<UpdateDrawingRevisionDto, DrawingRevision>();
    }
}
