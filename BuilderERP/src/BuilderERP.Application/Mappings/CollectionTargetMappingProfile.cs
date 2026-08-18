using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CollectionTargetMappingProfile : Profile
{
    public CollectionTargetMappingProfile()
    {
        CreateMap<CollectionTarget, CollectionTargetDto>()
            .ForMember(dest => dest.CollectionOfficerName, opt => opt.MapFrom(src => src.CollectionOfficer != null ? src.CollectionOfficer.FullName : string.Empty))
            .ForMember(dest => dest.ActualCollected, opt => opt.Ignore())
            .ForMember(dest => dest.AchievementPercent, opt => opt.Ignore());
        CreateMap<CreateCollectionTargetDto, CollectionTarget>();
        CreateMap<UpdateCollectionTargetDto, CollectionTarget>();
    }
}
