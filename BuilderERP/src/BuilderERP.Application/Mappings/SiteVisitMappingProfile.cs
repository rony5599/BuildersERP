using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SiteVisitMappingProfile : Profile
{
    public SiteVisitMappingProfile()
    {
        CreateMap<SiteVisit, SiteVisitDto>()
            .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead != null ? src.Lead.Name : string.Empty))
            .ForMember(dest => dest.PropertyUnitNumber, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : null))
            .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.FullName : null));
        CreateMap<CreateSiteVisitDto, SiteVisit>();
        CreateMap<UpdateSiteVisitDto, SiteVisit>();
    }
}
