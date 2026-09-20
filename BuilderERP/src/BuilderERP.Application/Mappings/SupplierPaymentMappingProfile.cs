using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class SupplierPaymentMappingProfile : Profile
{
    public SupplierPaymentMappingProfile()
    {
        CreateMap<SupplierPayment, SupplierPaymentDto>()
            .ForMember(d => d.PreparedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.BillNumber, o => o.MapFrom(s => s.PoBill != null ? s.PoBill.BillNumber : string.Empty))
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : string.Empty));
    }
}
