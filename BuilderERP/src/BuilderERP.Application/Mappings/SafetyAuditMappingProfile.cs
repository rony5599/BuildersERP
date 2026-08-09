using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SafetyAuditMappingProfile : Profile
{
    public SafetyAuditMappingProfile()
    {
        CreateMap<SafetyAudit, SafetyAuditDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateSafetyAuditDto, SafetyAudit>();
        CreateMap<UpdateSafetyAuditDto, SafetyAudit>();
    }
}
