using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class EquipmentRentalMappingProfile : Profile
{
    public EquipmentRentalMappingProfile()
    {
        CreateMap<EquipmentRental, EquipmentRentalDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.Name : string.Empty))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateEquipmentRentalDto, EquipmentRental>();
        CreateMap<UpdateEquipmentRentalDto, EquipmentRental>();
    }
}
