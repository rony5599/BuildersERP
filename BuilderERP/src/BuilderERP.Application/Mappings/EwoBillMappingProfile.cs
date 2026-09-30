using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class EwoBillMappingProfile : Profile
{
    public EwoBillMappingProfile()
    {
        CreateMap<EwoBill, EwoBillDto>()
            .ForMember(d => d.PreparedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.WorkOrderNo, o => o.MapFrom(s => s.EngineerWorkOrder != null ? s.EngineerWorkOrder.WorkOrderNo : string.Empty))
            .ForMember(d => d.ProjectName, o => o.MapFrom(s => s.EngineerWorkOrder != null && s.EngineerWorkOrder.EngineerWorkOrderRequisition != null && s.EngineerWorkOrder.EngineerWorkOrderRequisition.Project != null
                ? s.EngineerWorkOrder.EngineerWorkOrderRequisition.Project.Name : string.Empty))
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : string.Empty))
            .ForMember(d => d.HeadsSummary, o => o.MapFrom(s => string.Join(", ", s.Heads.Select(h => h.HeadName + " " + h.ClaimPercent.ToString("0.##") + "%"))))
            .ForMember(d => d.PaidAmount, o => o.Ignore());
        CreateMap<EwoBillDetail, EwoBillDetailDto>()
            .ForMember(d => d.MaterialName, o => o.MapFrom(s => s.Material != null ? s.Material.Name : string.Empty));
        CreateMap<EwoBillHead, EwoBillHeadDto>();
        CreateMap<EwoBillAdjustment, EwoBillAdjustmentDto>();
    }
}
