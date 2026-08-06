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
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : string.Empty));
        CreateMap<CreateRfqDto, Rfq>();
        CreateMap<UpdateRfqDto, Rfq>();
    }
}
