using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class OvertimeMappingProfile : Profile
{
    public OvertimeMappingProfile()
    {
        CreateMap<Overtime, OvertimeDto>()
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : null));
        CreateMap<CreateOvertimeDto, Overtime>();
        CreateMap<UpdateOvertimeDto, Overtime>();
    }
}
