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
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.VendorQuotation != null && src.VendorQuotation.Rfq != null && src.VendorQuotation.Rfq.Supplier != null ? src.VendorQuotation.Rfq.Supplier.Name : string.Empty));
        CreateMap<CreatePurchaseOrderDto, PurchaseOrder>();
        CreateMap<UpdatePurchaseOrderDto, PurchaseOrder>();
    }
}
