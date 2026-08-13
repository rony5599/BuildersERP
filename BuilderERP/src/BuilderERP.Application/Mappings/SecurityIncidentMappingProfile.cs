using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SecurityIncidentMappingProfile : Profile
{
    public SecurityIncidentMappingProfile()
    {
        CreateMap<SecurityIncident, SecurityIncidentDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateSecurityIncidentDto, SecurityIncident>();
        CreateMap<UpdateSecurityIncidentDto, SecurityIncident>();
    }
}
