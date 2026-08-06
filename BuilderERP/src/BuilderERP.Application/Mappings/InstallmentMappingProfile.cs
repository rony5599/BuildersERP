using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class InstallmentMappingProfile : Profile
{
    public InstallmentMappingProfile()
    {
        CreateMap<Installment, InstallmentDto>();
        CreateMap<CreateInstallmentDto, Installment>();
        CreateMap<UpdateInstallmentDto, Installment>();
    }
}
