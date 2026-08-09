using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PpeTrackingMappingProfile : Profile
{
    public PpeTrackingMappingProfile()
    {
        CreateMap<PpeTracking, PpeTrackingDto>()
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.Name : string.Empty));
        CreateMap<CreatePpeTrackingDto, PpeTracking>();
        CreateMap<UpdatePpeTrackingDto, PpeTracking>();
    }
}
