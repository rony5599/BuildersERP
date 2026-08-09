using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class IncidentReportMappingProfile : Profile
{
    public IncidentReportMappingProfile()
    {
        CreateMap<IncidentReport, IncidentReportDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateIncidentReportDto, IncidentReport>();
        CreateMap<UpdateIncidentReportDto, IncidentReport>();
    }
}
