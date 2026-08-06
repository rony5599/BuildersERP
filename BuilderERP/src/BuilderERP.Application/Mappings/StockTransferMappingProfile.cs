using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class StockTransferMappingProfile : Profile
{
    public StockTransferMappingProfile()
    {
        CreateMap<StockTransfer, StockTransferDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty))
            .ForMember(dest => dest.FromWarehouseName, opt => opt.MapFrom(src => src.FromWarehouse != null ? src.FromWarehouse.Name : string.Empty))
            .ForMember(dest => dest.ToWarehouseName, opt => opt.MapFrom(src => src.ToWarehouse != null ? src.ToWarehouse.Name : string.Empty));
        CreateMap<CreateStockTransferDto, StockTransfer>();
        CreateMap<UpdateStockTransferDto, StockTransfer>();
    }
}
