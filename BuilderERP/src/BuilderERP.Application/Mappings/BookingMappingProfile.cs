using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
            .ForMember(dest => dest.PropertyUnitNumber, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty))
            .ForMember(dest => dest.BrokerName, opt => opt.MapFrom(src => src.Broker != null ? src.Broker.Name : null))
            .ForMember(dest => dest.CollectionOfficerName, opt => opt.MapFrom(src => src.CollectionOfficer != null ? src.CollectionOfficer.FullName : null));
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<UpdateBookingDto, Booking>();
    }
}
