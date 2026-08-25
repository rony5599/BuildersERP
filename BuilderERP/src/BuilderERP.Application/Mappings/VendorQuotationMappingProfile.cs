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
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
            .ForMember(dest => dest.PurchaseRequisitionId, opt => opt.MapFrom(src => src.Rfq != null ? src.Rfq.PurchaseRequisitionId : 0L))
            .ForMember(dest => dest.RequisitionNumber, opt => opt.MapFrom(src => src.Rfq != null && src.Rfq.PurchaseRequisition != null ? src.Rfq.PurchaseRequisition.RequisitionNumber : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Rfq != null && src.Rfq.PurchaseRequisition != null ? src.Rfq.PurchaseRequisition.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Rfq != null && src.Rfq.PurchaseRequisition != null && src.Rfq.PurchaseRequisition.Project != null ? src.Rfq.PurchaseRequisition.Project.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<VendorQuotationDetail, VendorQuotationDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateVendorQuotationDto, VendorQuotation>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
