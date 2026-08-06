using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class QuotationMappingProfile : Profile
{
    public QuotationMappingProfile()
    {
        CreateMap<Quotation, QuotationDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
            .ForMember(dest => dest.PropertyUnitNumber, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateQuotationDto, Quotation>();
        CreateMap<UpdateQuotationDto, Quotation>();
    }
}
