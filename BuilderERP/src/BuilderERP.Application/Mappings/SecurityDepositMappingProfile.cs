using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SecurityDepositMappingProfile : Profile
{
    public SecurityDepositMappingProfile()
    {
        CreateMap<SecurityDeposit, SecurityDepositDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : string.Empty))
            .ForMember(dest => dest.WorkOrderNumber, opt => opt.MapFrom(src => src.WorkOrder != null ? src.WorkOrder.WorkOrderNumber : null));
        CreateMap<CreateSecurityDepositDto, SecurityDeposit>();
        CreateMap<UpdateSecurityDepositDto, SecurityDeposit>();
    }
}
