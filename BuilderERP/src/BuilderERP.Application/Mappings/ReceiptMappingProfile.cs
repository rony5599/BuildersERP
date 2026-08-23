using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ReceiptMappingProfile : Profile
{
    public ReceiptMappingProfile()
    {
        CreateMap<Receipt, ReceiptDto>()
            .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.Installment != null ? src.Installment.InstallmentNumber : 0))
            .ForMember(dest => dest.AgreementNumber, opt => opt.MapFrom(src => src.Installment != null && src.Installment.InstallmentPlan != null && src.Installment.InstallmentPlan.SaleAgreement != null ? src.Installment.InstallmentPlan.SaleAgreement.AgreementNumber : string.Empty))
            .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.Installment != null && src.Installment.InstallmentPlan != null && src.Installment.InstallmentPlan.SaleAgreement != null && src.Installment.InstallmentPlan.SaleAgreement.Booking != null && src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit != null ? src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.UnitNumber : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Installment != null && src.Installment.InstallmentPlan != null && src.Installment.InstallmentPlan.SaleAgreement != null && src.Installment.InstallmentPlan.SaleAgreement.Booking != null && src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit != null && src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor != null && src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower != null && src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building != null && src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.Project != null ? src.Installment.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.Project.Name : string.Empty));
        CreateMap<CreateReceiptDto, Receipt>();
        CreateMap<UpdateReceiptDto, Receipt>();
    }
}
