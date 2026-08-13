using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SnagItemMappingProfile : Profile
{
    public SnagItemMappingProfile()
    {
        CreateMap<SnagItem, SnagItemDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateSnagItemDto, SnagItem>();
        CreateMap<UpdateSnagItemDto, SnagItem>();
    }
}
