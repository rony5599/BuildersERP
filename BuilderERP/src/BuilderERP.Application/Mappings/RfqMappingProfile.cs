using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class RfqMappingProfile : Profile
{
    public RfqMappingProfile()
    {
        CreateMap<Rfq, RfqDto>()
            .ForMember(dest => dest.RequisitionNumber, opt => opt.MapFrom(src => src.PurchaseRequisition != null ? src.PurchaseRequisition.RequisitionNumber : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.PurchaseRequisition != null ? src.PurchaseRequisition.ProjectId : 0L))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.PurchaseRequisition != null && src.PurchaseRequisition.Project != null ? src.PurchaseRequisition.Project.Name : string.Empty))
            .ForMember(dest => dest.Vendors, opt => opt.MapFrom(src => src.RfqVendors))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<RfqVendor, RfqVendorDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty));
        CreateMap<RfqDetail, RfqDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateRfqDto, Rfq>()
            .ForMember(dest => dest.RfqVendors, opt => opt.Ignore())
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
