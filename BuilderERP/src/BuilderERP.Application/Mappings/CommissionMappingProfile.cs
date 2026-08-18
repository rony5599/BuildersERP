using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CommissionMappingProfile : Profile
{
    public CommissionMappingProfile()
    {
        CreateMap<Commission, CommissionDto>()
            .ForMember(dest => dest.BrokerName, opt => opt.MapFrom(src => src.Broker != null ? src.Broker.Name : string.Empty))
            .ForMember(dest => dest.BookingAmount, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.BookingAmount : 0m));
        CreateMap<CreateCommissionDto, Commission>()
            .ForMember(dest => dest.CommissionAmount, opt => opt.Ignore());
        CreateMap<UpdateCommissionDto, Commission>()
            .ForMember(dest => dest.CommissionAmount, opt => opt.Ignore());
    }
}
