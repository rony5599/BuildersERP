using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class MaintenanceRequestMappingProfile : Profile
{
    public MaintenanceRequestMappingProfile()
    {
        CreateMap<MaintenanceRequest, MaintenanceRequestDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateMaintenanceRequestDto, MaintenanceRequest>();
        CreateMap<UpdateMaintenanceRequestDto, MaintenanceRequest>();
    }
}
