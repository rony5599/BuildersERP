using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class ItemCategoryMappingProfile : Profile
{
    public ItemCategoryMappingProfile()
    {
        CreateMap<ItemCategory, ItemCategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : string.Empty));
        CreateMap<CreateItemCategoryDto, ItemCategory>();
        CreateMap<UpdateItemCategoryDto, ItemCategory>();
    }
}
