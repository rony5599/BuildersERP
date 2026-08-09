using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class WorkerMappingProfile : Profile
{
    public WorkerMappingProfile()
    {
        CreateMap<Worker, WorkerDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : null));
        CreateMap<CreateWorkerDto, Worker>();
        CreateMap<UpdateWorkerDto, Worker>();
    }
}
