using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ContractorLedgerMappingProfile : Profile
{
    public ContractorLedgerMappingProfile()
    {
        CreateMap<ContractorLedger, ContractorLedgerDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : string.Empty));
        CreateMap<CreateContractorLedgerDto, ContractorLedger>();
        CreateMap<UpdateContractorLedgerDto, ContractorLedger>();
    }
}
