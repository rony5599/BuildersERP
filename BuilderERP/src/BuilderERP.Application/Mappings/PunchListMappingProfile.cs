using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PunchListMappingProfile : Profile
{
    public PunchListMappingProfile()
    {
        CreateMap<PunchList, PunchListDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreatePunchListDto, PunchList>();
        CreateMap<UpdatePunchListDto, PunchList>();
    }
}
