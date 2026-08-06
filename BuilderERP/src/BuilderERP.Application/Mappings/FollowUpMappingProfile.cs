using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class FollowUpMappingProfile : Profile
{
    public FollowUpMappingProfile()
    {
        CreateMap<FollowUp, FollowUpDto>()
            .ForMember(dest => dest.LeadName, opt => opt.MapFrom(src => src.Lead != null ? src.Lead.Name : string.Empty));
        CreateMap<CreateFollowUpDto, FollowUp>();
        CreateMap<UpdateFollowUpDto, FollowUp>();
    }
}
