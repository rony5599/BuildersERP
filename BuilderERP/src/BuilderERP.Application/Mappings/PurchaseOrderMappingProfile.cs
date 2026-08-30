using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PurchaseOrderMappingProfile : Profile
{
    public PurchaseOrderMappingProfile()
    {
        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .ForMember(dest => dest.QuotationNumber, opt => opt.MapFrom(src => src.VendorQuotation != null ? src.VendorQuotation.QuotationNumber : string.Empty))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.VendorQuotation != null && src.VendorQuotation.Supplier != null ? src.VendorQuotation.Supplier.Name : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.VendorQuotation != null && src.VendorQuotation.Rfq != null && src.VendorQuotation.Rfq.PurchaseRequisition != null ? src.VendorQuotation.Rfq.PurchaseRequisition.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.VendorQuotation != null && src.VendorQuotation.Rfq != null && src.VendorQuotation.Rfq.PurchaseRequisition != null && src.VendorQuotation.Rfq.PurchaseRequisition.Project != null ? src.VendorQuotation.Rfq.PurchaseRequisition.Project.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<PurchaseOrderDetail, PurchaseOrderDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreatePurchaseOrderDto, PurchaseOrder>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
