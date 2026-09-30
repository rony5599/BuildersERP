using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class EngineerWorkOrderMappingProfile : Profile
{
    public EngineerWorkOrderMappingProfile()
    {
        CreateMap<EngineerWorkOrder, EngineerWorkOrderDto>()
            .ForMember(dest => dest.RequisitionNumber, opt => opt.MapFrom(src => src.EngineerWorkOrderRequisition != null ? src.EngineerWorkOrderRequisition.RequisitionNumber : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.EngineerWorkOrderRequisition != null ? src.EngineerWorkOrderRequisition.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.EngineerWorkOrderRequisition != null && src.EngineerWorkOrderRequisition.Project != null ? src.EngineerWorkOrderRequisition.Project.Name : string.Empty))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
            .ForMember(dest => dest.PaymentHeads, opt => opt.MapFrom(src => src.PaymentHeads.OrderBy(h => h.SortOrder)));
        CreateMap<EngineerWorkOrderPaymentHead, EngineerWorkOrderPaymentHeadDto>();
        CreateMap<EngineerWorkOrderDetail, EngineerWorkOrderDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateEngineerWorkOrderDto, EngineerWorkOrder>()
            .ForMember(dest => dest.Details, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentHeads, opt => opt.Ignore());
    }
}
