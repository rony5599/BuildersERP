using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ReceiptMappingProfile : Profile
{
    public ReceiptMappingProfile()
    {
        CreateMap<Receipt, ReceiptDto>()
            .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.Installment != null ? src.Installment.InstallmentNumber : 0));
        CreateMap<CreateReceiptDto, Receipt>();
        CreateMap<UpdateReceiptDto, Receipt>();
    }
}
