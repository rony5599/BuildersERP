using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class UtilityBillMappingProfile : Profile
{
    public UtilityBillMappingProfile()
    {
        CreateMap<UtilityBill, UtilityBillDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateUtilityBillDto, UtilityBill>();
        CreateMap<UpdateUtilityBillDto, UtilityBill>();
    }
}
