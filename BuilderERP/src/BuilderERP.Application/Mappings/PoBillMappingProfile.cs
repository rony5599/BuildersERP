using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PoBillMappingProfile : Profile
{
    public PoBillMappingProfile()
    {
        CreateMap<PoBill, PoBillDto>()
            .ForMember(d => d.PreparedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.PONumber, o => o.MapFrom(s => s.PurchaseOrder != null ? s.PurchaseOrder.PONumber : string.Empty))
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.PurchaseOrder != null && s.PurchaseOrder.VendorQuotation != null && s.PurchaseOrder.VendorQuotation.Supplier != null ? s.PurchaseOrder.VendorQuotation.Supplier.Name : string.Empty))
            .ForMember(d => d.ProjectId, o => o.MapFrom(s => s.PurchaseOrder != null && s.PurchaseOrder.VendorQuotation != null && s.PurchaseOrder.VendorQuotation.Rfq != null && s.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition != null ? s.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId : 0L))
            .ForMember(d => d.ProjectName, o => o.MapFrom(s => s.PurchaseOrder != null && s.PurchaseOrder.VendorQuotation != null && s.PurchaseOrder.VendorQuotation.Rfq != null && s.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition != null && s.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.Project != null ? s.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.Project.Name : string.Empty))
            .ForMember(d => d.Details, o => o.MapFrom(s => s.Details));
        CreateMap<PoBillDetail, PoBillDetailDto>()
            .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material != null ? s.Material.Name : string.Empty));
    }
}
