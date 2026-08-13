using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class FlatHandoverMappingProfile : Profile
{
    public FlatHandoverMappingProfile()
    {
        CreateMap<FlatHandover, FlatHandoverDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty));
        CreateMap<CreateFlatHandoverDto, FlatHandover>();
        CreateMap<UpdateFlatHandoverDto, FlatHandover>();
    }
}
