using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class RateContractMappingProfile : Profile
{
    public RateContractMappingProfile()
    {
        CreateMap<RateContract, RateContractDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : null));
        CreateMap<CreateRateContractDto, RateContract>();
        CreateMap<UpdateRateContractDto, RateContract>();
    }
}
