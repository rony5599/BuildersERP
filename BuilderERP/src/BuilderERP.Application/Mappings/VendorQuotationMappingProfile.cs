using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class VendorQuotationMappingProfile : Profile
{
    public VendorQuotationMappingProfile()
    {
        CreateMap<VendorQuotation, VendorQuotationDto>()
            .ForMember(dest => dest.RfqNumber, opt => opt.MapFrom(src => src.Rfq != null ? src.Rfq.RfqNumber : string.Empty))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Rfq != null && src.Rfq.Supplier != null ? src.Rfq.Supplier.Name : string.Empty))
            .ForMember(dest => dest.PurchaseRequisitionId, opt => opt.MapFrom(src => src.Rfq != null ? src.Rfq.PurchaseRequisitionId : Guid.Empty))
            .ForMember(dest => dest.RequisitionNumber, opt => opt.MapFrom(src => src.Rfq != null && src.Rfq.PurchaseRequisition != null ? src.Rfq.PurchaseRequisition.RequisitionNumber : string.Empty));
        CreateMap<CreateVendorQuotationDto, VendorQuotation>();
        CreateMap<UpdateVendorQuotationDto, VendorQuotation>();
    }
}
