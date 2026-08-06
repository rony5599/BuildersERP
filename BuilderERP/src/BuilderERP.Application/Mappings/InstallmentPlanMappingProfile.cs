using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class InstallmentPlanMappingProfile : Profile
{
    public InstallmentPlanMappingProfile()
    {
        CreateMap<InstallmentPlan, InstallmentPlanDto>()
            .ForMember(dest => dest.SaleAgreementNumber, opt => opt.MapFrom(src => src.SaleAgreement != null ? src.SaleAgreement.AgreementNumber : string.Empty));
        CreateMap<CreateInstallmentPlanDto, InstallmentPlan>();
        CreateMap<UpdateInstallmentPlanDto, InstallmentPlan>();
    }
}
