using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SaleAgreementMappingProfile : Profile
{
    public SaleAgreementMappingProfile()
    {
        CreateMap<SaleAgreement, SaleAgreementDto>()
            .ForMember(dest => dest.BookingUnitNumber, opt => opt.MapFrom(src => src.Booking != null && src.Booking.PropertyUnit != null ? src.Booking.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateSaleAgreementDto, SaleAgreement>();
        CreateMap<UpdateSaleAgreementDto, SaleAgreement>();
    }
}
