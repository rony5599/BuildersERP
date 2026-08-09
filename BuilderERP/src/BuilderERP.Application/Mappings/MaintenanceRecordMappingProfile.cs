using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class MaintenanceRecordMappingProfile : Profile
{
    public MaintenanceRecordMappingProfile()
    {
        CreateMap<MaintenanceRecord, MaintenanceRecordDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.Name : string.Empty));
        CreateMap<CreateMaintenanceRecordDto, MaintenanceRecord>();
        CreateMap<UpdateMaintenanceRecordDto, MaintenanceRecord>();
    }
}
