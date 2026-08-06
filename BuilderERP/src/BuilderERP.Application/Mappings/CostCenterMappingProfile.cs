using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CostCenterMappingProfile : Profile
{
    public CostCenterMappingProfile()
    {
        CreateMap<CostCenter, CostCenterDto>();
        CreateMap<CreateCostCenterDto, CostCenter>();
        CreateMap<UpdateCostCenterDto, CostCenter>();
    }
}
