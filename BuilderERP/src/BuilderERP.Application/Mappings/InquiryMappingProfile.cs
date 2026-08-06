using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class InquiryMappingProfile : Profile
{
    public InquiryMappingProfile()
    {
        CreateMap<Inquiry, InquiryDto>()
            .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead != null ? src.Lead.Name : string.Empty))
            .ForMember(dest => dest.PropertyUnitNumber, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : null));
        CreateMap<CreateInquiryDto, Inquiry>();
        CreateMap<UpdateInquiryDto, Inquiry>();
    }
}
