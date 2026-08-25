using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class GoodsReceiveMappingProfile : Profile
{
    public GoodsReceiveMappingProfile()
    {
        CreateMap<GoodsReceive, GoodsReceiveDto>()
            .ForMember(dest => dest.PONumber, opt => opt.MapFrom(src => src.PurchaseOrder != null ? src.PurchaseOrder.PONumber : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.PurchaseOrder != null && src.PurchaseOrder.VendorQuotation != null && src.PurchaseOrder.VendorQuotation.Rfq != null && src.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition != null ? src.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.PurchaseOrder != null && src.PurchaseOrder.VendorQuotation != null && src.PurchaseOrder.VendorQuotation.Rfq != null && src.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition != null && src.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.Project != null ? src.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.Project.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<GoodsReceiveDetail, GoodsReceiveDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateGoodsReceiveDto, GoodsReceive>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
