using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ContractorMappingProfile : Profile
{
    public ContractorMappingProfile()
    {
        CreateMap<Contractor, ContractorDto>();
        CreateMap<CreateContractorDto, Contractor>();
        CreateMap<UpdateContractorDto, Contractor>();
    }
}
