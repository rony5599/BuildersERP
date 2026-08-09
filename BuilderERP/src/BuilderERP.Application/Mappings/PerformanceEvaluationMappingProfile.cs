using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PerformanceEvaluationMappingProfile : Profile
{
    public PerformanceEvaluationMappingProfile()
    {
        CreateMap<PerformanceEvaluation, PerformanceEvaluationDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : null))
            .ForMember(dest => dest.OverallScore, opt => opt.MapFrom(src => Math.Round((src.QualityScore + src.TimelinessScore + src.SafetyScore) / 3.0, 2)));
        CreateMap<CreatePerformanceEvaluationDto, PerformanceEvaluation>();
        CreateMap<UpdatePerformanceEvaluationDto, PerformanceEvaluation>();
    }
}
