using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class InstallmentPlanMappingProfile : Profile
{
    public InstallmentPlanMappingProfile()
    {
        CreateMap<InstallmentPlan, InstallmentPlanDto>()
            .ForMember(dest => dest.SaleAgreementNumber, opt => opt.MapFrom(src => src.SaleAgreement != null ? src.SaleAgreement.AgreementNumber : string.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.SaleAgreement != null && src.SaleAgreement.Booking != null && src.SaleAgreement.Booking.Customer != null ? src.SaleAgreement.Booking.Customer.FullName : string.Empty))
            .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.SaleAgreement != null && src.SaleAgreement.Booking != null && src.SaleAgreement.Booking.PropertyUnit != null ? src.SaleAgreement.Booking.PropertyUnit.UnitNumber : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.SaleAgreement != null && src.SaleAgreement.Booking != null && src.SaleAgreement.Booking.PropertyUnit != null ? src.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.ProjectId : (Guid?)null))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.SaleAgreement != null && src.SaleAgreement.Booking != null && src.SaleAgreement.Booking.PropertyUnit != null && src.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.Project != null ? src.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.Project.Name : string.Empty));
        CreateMap<CreateInstallmentPlanDto, InstallmentPlan>();
        CreateMap<UpdateInstallmentPlanDto, InstallmentPlan>();
    }
}
