using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CashDisbursementMappingProfile : Profile
{
    public CashDisbursementMappingProfile()
    {
        CreateMap<CashDisbursement, CashDisbursementDto>()
            .ForMember(d => d.PreparedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.RequisitionNumber, o => o.MapFrom(s => s.CashRequisition != null ? s.CashRequisition.RequisitionNumber : string.Empty))
            .ForMember(d => d.RequesterName, o => o.MapFrom(s => s.RequesterEmployee != null ? s.RequesterEmployee.EmployeeName : string.Empty));
    }
}
