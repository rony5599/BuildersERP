using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class EngineerWorkOrderRequisitionMappingProfile : Profile
{
    public EngineerWorkOrderRequisitionMappingProfile()
    {
        CreateMap<EngineerWorkOrderRequisition, EngineerWorkOrderRequisitionDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<EngineerWorkOrderRequisitionDetail, EngineerWorkOrderRequisitionDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateEngineerWorkOrderRequisitionDto, EngineerWorkOrderRequisition>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
