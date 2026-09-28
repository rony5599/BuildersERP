using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CashPoBillMappingProfile : Profile
{
    public CashPoBillMappingProfile()
    {
        CreateMap<CashPoBill, CashPoBillDto>()
            .ForMember(d => d.PreparedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.CPONumber, o => o.MapFrom(s => s.CashPurchaseOrder != null ? s.CashPurchaseOrder.CPONumber : string.Empty))
            .ForMember(d => d.RequisitionNumber, o => o.MapFrom(s => s.CashPurchaseOrder != null && s.CashPurchaseOrder.CashRequisition != null ? s.CashPurchaseOrder.CashRequisition.RequisitionNumber : string.Empty))
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.CashPurchaseOrder != null && s.CashPurchaseOrder.Supplier != null ? s.CashPurchaseOrder.Supplier.Name : string.Empty))
            .ForMember(d => d.RequesterName, o => o.MapFrom(s => s.RequesterEmployee != null ? s.RequesterEmployee.EmployeeName : string.Empty))
            .ForMember(d => d.Details, o => o.MapFrom(s => s.Details));
        CreateMap<CashPoBillDetail, CashPoBillDetailDto>()
            .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material != null ? s.Material.Name : string.Empty));
    }
}
