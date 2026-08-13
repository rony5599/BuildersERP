using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ApartmentMaintenanceMappingProfile : Profile
{
    public ApartmentMaintenanceMappingProfile()
    {
        CreateMap<ApartmentMaintenance, ApartmentMaintenanceDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateApartmentMaintenanceDto, ApartmentMaintenance>();
        CreateMap<UpdateApartmentMaintenanceDto, ApartmentMaintenance>();
    }
}
