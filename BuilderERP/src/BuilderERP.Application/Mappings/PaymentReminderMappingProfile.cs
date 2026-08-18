using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class PaymentReminderMappingProfile : Profile
{
    public PaymentReminderMappingProfile()
    {
        CreateMap<PaymentReminder, PaymentReminderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : string.Empty))
            .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.Installment != null ? src.Installment.InstallmentNumber : (int?)null))
            .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.Installment != null ? src.Installment.DueAmount + src.Installment.PenaltyAmount - src.Installment.PaidAmount : (decimal?)null))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.Installment != null ? src.Installment.DueDate : (DateTime?)null));
    }
}
