using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class WarrantyMappingProfile : Profile
{
    public WarrantyMappingProfile()
    {
        CreateMap<Warranty, WarrantyDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateWarrantyDto, Warranty>();
        CreateMap<UpdateWarrantyDto, Warranty>();
    }
}
