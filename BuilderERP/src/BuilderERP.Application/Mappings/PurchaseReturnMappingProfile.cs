using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PurchaseReturnMappingProfile : Profile
{
    public PurchaseReturnMappingProfile()
    {
        CreateMap<PurchaseReturn, PurchaseReturnDto>()
            .ForMember(dest => dest.GrnNumber, opt => opt.MapFrom(src => src.GoodsReceive != null ? src.GoodsReceive.GrnNumber : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<PurchaseReturnDetail, PurchaseReturnDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreatePurchaseReturnDto, PurchaseReturn>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
