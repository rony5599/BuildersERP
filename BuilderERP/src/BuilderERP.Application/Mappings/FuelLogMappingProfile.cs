using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class FuelLogMappingProfile : Profile
{
    public FuelLogMappingProfile()
    {
        CreateMap<FuelLog, FuelLogDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.Name : string.Empty));
        CreateMap<CreateFuelLogDto, FuelLog>();
        CreateMap<UpdateFuelLogDto, FuelLog>();
    }
}
