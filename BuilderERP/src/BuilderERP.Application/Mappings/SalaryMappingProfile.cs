using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SalaryMappingProfile : Profile
{
    public SalaryMappingProfile()
    {
        CreateMap<Salary, SalaryDto>()
            .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.Worker != null ? src.Worker.Name : string.Empty));
        CreateMap<CreateSalaryDto, Salary>();
        CreateMap<UpdateSalaryDto, Salary>();
    }
}
