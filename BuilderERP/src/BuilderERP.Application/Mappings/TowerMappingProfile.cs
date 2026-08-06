using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class TowerMappingProfile : Profile
{
    public TowerMappingProfile()
    {
        CreateMap<Tower, TowerDto>();
        CreateMap<CreateTowerDto, Tower>();
        CreateMap<UpdateTowerDto, Tower>();
    }
}
