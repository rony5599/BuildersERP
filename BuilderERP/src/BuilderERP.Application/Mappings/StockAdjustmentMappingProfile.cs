using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class StockAdjustmentMappingProfile : Profile
{
    public StockAdjustmentMappingProfile()
    {
        CreateMap<StockAdjustment, StockAdjustmentDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.ProjectId : Guid.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Warehouse != null && src.Warehouse.Project != null ? src.Warehouse.Project.Name : string.Empty));
        CreateMap<CreateStockAdjustmentDto, StockAdjustment>();
        CreateMap<UpdateStockAdjustmentDto, StockAdjustment>();
    }
}
