using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class BuildingMappingProfile : Profile
{
    public BuildingMappingProfile()
    {
        CreateMap<Building, BuildingDto>();
        CreateMap<CreateBuildingDto, Building>();
        CreateMap<UpdateBuildingDto, Building>();
    }
}
