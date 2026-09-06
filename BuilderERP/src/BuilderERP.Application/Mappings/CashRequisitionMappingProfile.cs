using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CashRequisitionMappingProfile : Profile
{
    public CashRequisitionMappingProfile()
    {
        CreateMap<CashRequisition, CashRequisitionDto>()
            .ForMember(dest => dest.RequesterEmployeeName, opt => opt.MapFrom(src => src.RequesterEmployee != null ? src.RequesterEmployee.EmployeeName : string.Empty))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));
        CreateMap<CashRequisitionDetail, CashRequisitionDetailDto>()
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : string.Empty));
        CreateMap<CreateCashRequisitionDto, CashRequisition>()
            .ForMember(dest => dest.Details, opt => opt.Ignore());
    }
}
