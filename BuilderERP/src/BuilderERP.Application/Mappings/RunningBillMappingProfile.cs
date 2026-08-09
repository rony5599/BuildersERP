using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class RunningBillMappingProfile : Profile
{
    public RunningBillMappingProfile()
    {
        CreateMap<RunningBill, RunningBillDto>()
            .ForMember(dest => dest.WorkOrderNumber, opt => opt.MapFrom(src => src.WorkOrder != null ? src.WorkOrder.WorkOrderNumber : string.Empty));
        CreateMap<CreateRunningBillDto, RunningBill>();
        CreateMap<UpdateRunningBillDto, RunningBill>();
    }
}
