using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class RiskAssessmentMappingProfile : Profile
{
    public RiskAssessmentMappingProfile()
    {
        CreateMap<RiskAssessment, RiskAssessmentDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateRiskAssessmentDto, RiskAssessment>();
        CreateMap<UpdateRiskAssessmentDto, RiskAssessment>();
    }
}
