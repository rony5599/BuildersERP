using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class StockReturnMappingProfile : Profile
{
    public StockReturnMappingProfile()
    {
        CreateMap<StockReturn, StockReturnDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty));
        CreateMap<CreateStockReturnDto, StockReturn>();
        CreateMap<UpdateStockReturnDto, StockReturn>();
    }
}
