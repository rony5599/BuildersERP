using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PurchaseRequisitionMappingProfile : Profile
{
    public PurchaseRequisitionMappingProfile()
    {
        CreateMap<PurchaseRequisition, PurchaseRequisitionDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<PurchaseRequisitionDetail, PurchaseRequisitionDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreatePurchaseRequisitionDto, PurchaseRequisition>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
