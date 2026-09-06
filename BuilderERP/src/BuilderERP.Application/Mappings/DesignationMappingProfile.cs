using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DesignationMappingProfile : Profile
{
    public DesignationMappingProfile()
    {
        CreateMap<Designation, DesignationDto>();
        CreateMap<CreateDesignationDto, Designation>();
        CreateMap<UpdateDesignationDto, Designation>();
    }
}
