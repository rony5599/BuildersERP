using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ParkingSlotMappingProfile : Profile
{
    public ParkingSlotMappingProfile()
    {
        CreateMap<ParkingSlot, ParkingSlotDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateParkingSlotDto, ParkingSlot>();
        CreateMap<UpdateParkingSlotDto, ParkingSlot>();
    }
}
