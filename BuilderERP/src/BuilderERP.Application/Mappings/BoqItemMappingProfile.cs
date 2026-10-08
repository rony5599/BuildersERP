using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class BoqItemMappingProfile : Profile
{
    public BoqItemMappingProfile()
    {
        CreateMap<BoqItem, BoqItemDto>()
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Boq.ProjectId))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Boq.Project.Name))
            .ForMember(dest => dest.BoqName, opt => opt.MapFrom(src => src.Boq.BoqName))
            .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.Boq.VersionNumber))
            .ForMember(dest => dest.WorkGroupCode, opt => opt.MapFrom(src => src.WorkGroup.GroupCode))
            .ForMember(dest => dest.WorkGroupName, opt => opt.MapFrom(src => src.WorkGroup.GroupName));
        CreateMap<CreateBoqItemDto, BoqItem>();
        CreateMap<UpdateBoqItemDto, BoqItem>();
    }
}
