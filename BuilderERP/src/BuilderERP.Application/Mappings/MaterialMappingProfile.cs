using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class MaterialMappingProfile : Profile
{
    public MaterialMappingProfile()
    {
        CreateMap<Material, MaterialDto>();
        CreateMap<CreateMaterialDto, Material>();
        CreateMap<UpdateMaterialDto, Material>();
    }
}
