using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CashPurchaseOrderMappingProfile : Profile
{
    public CashPurchaseOrderMappingProfile()
    {
        CreateMap<CashPurchaseOrder, CashPurchaseOrderDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
            .ForMember(dest => dest.RequisitionNumber, opt => opt.MapFrom(src => src.CashRequisition != null ? src.CashRequisition.RequisitionNumber : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.CashRequisition != null ? src.CashRequisition.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.CashRequisition != null && src.CashRequisition.Project != null ? src.CashRequisition.Project.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<CashPurchaseOrderDetail, CashPurchaseOrderDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateCashPurchaseOrderDto, CashPurchaseOrder>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
