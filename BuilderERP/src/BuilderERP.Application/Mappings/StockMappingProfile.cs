using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class StockMappingProfile : Profile
{
    public StockMappingProfile()
    {
        CreateMap<Stock, StockDto>()
            .ForMember(dest => dest.MaterialCode, opt => opt.MapFrom(src => src.Material != null ? src.Material.MaterialCode : string.Empty))
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty))
            .ForMember(dest => dest.ReorderLevel, opt => opt.MapFrom(src => src.Material != null ? src.Material.ReorderLevel : 0))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.IsBelowReorderLevel, opt => opt.MapFrom(src => src.Material != null && src.QuantityOnHand < src.Material.ReorderLevel))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Warehouse != null && src.Warehouse.Project != null ? src.Warehouse.Project.Name : string.Empty));
    }
}
