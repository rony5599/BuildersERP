using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class GoodsReceiveMappingProfile : Profile
{
    public GoodsReceiveMappingProfile()
    {
        CreateMap<GoodsReceive, GoodsReceiveDto>()
            .ForMember(dest => dest.PONumber, opt => opt.MapFrom(src => src.PurchaseOrder != null ? src.PurchaseOrder.PONumber : string.Empty));
        CreateMap<CreateGoodsReceiveDto, GoodsReceive>();
        CreateMap<UpdateGoodsReceiveDto, GoodsReceive>();
    }
}
