using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PurchaseReturnMappingProfile : Profile
{
    public PurchaseReturnMappingProfile()
    {
        CreateMap<PurchaseReturn, PurchaseReturnDto>()
            .ForMember(dest => dest.GrnNumber, opt => opt.MapFrom(src => src.GoodsReceive != null ? src.GoodsReceive.GrnNumber : string.Empty));
        CreateMap<CreatePurchaseReturnDto, PurchaseReturn>();
        CreateMap<UpdatePurchaseReturnDto, PurchaseReturn>();
    }
}
