using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DrawingApprovalMappingProfile : Profile
{
    public DrawingApprovalMappingProfile()
    {
        CreateMap<DrawingApproval, DrawingApprovalDto>()
            .ForMember(dest => dest.DrawingNumber, opt => opt.MapFrom(src => src.Drawing != null ? src.Drawing.DrawingNumber : string.Empty))
            .ForMember(dest => dest.RevisionCode, opt => opt.MapFrom(src => src.DrawingRevision != null ? src.DrawingRevision.RevisionCode : null));
        CreateMap<CreateDrawingApprovalDto, DrawingApproval>();
        CreateMap<UpdateDrawingApprovalDto, DrawingApproval>();
    }
}
