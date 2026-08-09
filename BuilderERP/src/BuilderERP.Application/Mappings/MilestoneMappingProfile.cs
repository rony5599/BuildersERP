using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class MilestoneMappingProfile : Profile
{
    public MilestoneMappingProfile()
    {
        CreateMap<Milestone, MilestoneDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateMilestoneDto, Milestone>();
        CreateMap<UpdateMilestoneDto, Milestone>();
    }
}
