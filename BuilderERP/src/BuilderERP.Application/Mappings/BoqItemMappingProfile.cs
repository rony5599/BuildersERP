using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class BoqItemMappingProfile : Profile
{
    public BoqItemMappingProfile()
    {
        CreateMap<BoqItem, BoqItemDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateBoqItemDto, BoqItem>();
        CreateMap<UpdateBoqItemDto, BoqItem>();
    }
}
