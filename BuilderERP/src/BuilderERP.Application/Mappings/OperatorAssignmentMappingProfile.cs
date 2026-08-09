using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class OperatorAssignmentMappingProfile : Profile
{
    public OperatorAssignmentMappingProfile()
    {
        CreateMap<OperatorAssignment, OperatorAssignmentDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment != null ? src.Equipment.Name : string.Empty))
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateOperatorAssignmentDto, OperatorAssignment>();
        CreateMap<UpdateOperatorAssignmentDto, OperatorAssignment>();
    }
}
