using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PurchaseRequisitionMappingProfile : Profile
{
    public PurchaseRequisitionMappingProfile()
    {
        CreateMap<PurchaseRequisition, PurchaseRequisitionDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty));
        CreateMap<CreatePurchaseRequisitionDto, PurchaseRequisition>();
        CreateMap<UpdatePurchaseRequisitionDto, PurchaseRequisition>();
    }
}
