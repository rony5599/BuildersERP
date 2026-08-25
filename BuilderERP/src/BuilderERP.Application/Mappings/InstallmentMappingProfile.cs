using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class InstallmentMappingProfile : Profile
{
    public InstallmentMappingProfile()
    {
        CreateMap<Installment, InstallmentDto>()
            .ForMember(dest => dest.AgreementNumber, opt => opt.MapFrom(src => src.InstallmentPlan != null && src.InstallmentPlan.SaleAgreement != null ? src.InstallmentPlan.SaleAgreement.AgreementNumber : string.Empty))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.InstallmentPlan != null && src.InstallmentPlan.SaleAgreement != null && src.InstallmentPlan.SaleAgreement.Booking != null && src.InstallmentPlan.SaleAgreement.Booking.Customer != null ? src.InstallmentPlan.SaleAgreement.Booking.Customer.FullName : string.Empty))
            .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.InstallmentPlan != null && src.InstallmentPlan.SaleAgreement != null && src.InstallmentPlan.SaleAgreement.Booking != null && src.InstallmentPlan.SaleAgreement.Booking.PropertyUnit != null ? src.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.UnitNumber : string.Empty))
            .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.InstallmentPlan != null && src.InstallmentPlan.SaleAgreement != null && src.InstallmentPlan.SaleAgreement.Booking != null && src.InstallmentPlan.SaleAgreement.Booking.PropertyUnit != null ? src.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.ProjectId : (long?)null))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.InstallmentPlan != null && src.InstallmentPlan.SaleAgreement != null && src.InstallmentPlan.SaleAgreement.Booking != null && src.InstallmentPlan.SaleAgreement.Booking.PropertyUnit != null && src.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.Project != null ? src.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.Project.Name : string.Empty));
        CreateMap<CreateInstallmentDto, Installment>();
        CreateMap<UpdateInstallmentDto, Installment>();
    }
}
