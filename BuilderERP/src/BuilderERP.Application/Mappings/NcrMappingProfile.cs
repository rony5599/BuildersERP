using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class NcrMappingProfile : Profile
{
    public NcrMappingProfile()
    {
        CreateMap<Ncr, NcrDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateNcrDto, Ncr>();
        CreateMap<UpdateNcrDto, Ncr>();
    }
}
