using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class QualityChecklistMappingProfile : Profile
{
    public QualityChecklistMappingProfile()
    {
        CreateMap<QualityChecklist, QualityChecklistDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateQualityChecklistDto, QualityChecklist>();
        CreateMap<UpdateQualityChecklistDto, QualityChecklist>();
    }
}
