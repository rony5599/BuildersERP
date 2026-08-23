using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SaleAgreementMappingProfile : Profile
{
    public SaleAgreementMappingProfile()
    {
        CreateMap<SaleAgreement, SaleAgreementDto>()
            .ForMember(dest => dest.BookingUnitNumber, opt => opt.MapFrom(src => src.Booking != null && src.Booking.PropertyUnit != null ? src.Booking.PropertyUnit.UnitNumber : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Booking != null && src.Booking.PropertyUnit != null && src.Booking.PropertyUnit.Floor != null && src.Booking.PropertyUnit.Floor.Tower != null && src.Booking.PropertyUnit.Floor.Tower.Building != null ? src.Booking.PropertyUnit.Floor.Tower.Building.ProjectId : Guid.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Booking != null && src.Booking.PropertyUnit != null && src.Booking.PropertyUnit.Floor != null && src.Booking.PropertyUnit.Floor.Tower != null && src.Booking.PropertyUnit.Floor.Tower.Building != null && src.Booking.PropertyUnit.Floor.Tower.Building.Project != null ? src.Booking.PropertyUnit.Floor.Tower.Building.Project.Name : string.Empty));
        CreateMap<CreateSaleAgreementDto, SaleAgreement>();
        CreateMap<UpdateSaleAgreementDto, SaleAgreement>();
    }
}
