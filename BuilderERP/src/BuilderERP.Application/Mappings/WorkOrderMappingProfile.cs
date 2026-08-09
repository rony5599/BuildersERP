using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class WorkOrderMappingProfile : Profile
{
    public WorkOrderMappingProfile()
    {
        CreateMap<WorkOrder, WorkOrderDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateWorkOrderDto, WorkOrder>();
        CreateMap<UpdateWorkOrderDto, WorkOrder>();
    }
}
