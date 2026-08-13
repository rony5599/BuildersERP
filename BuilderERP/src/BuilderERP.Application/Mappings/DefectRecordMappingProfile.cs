using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DefectRecordMappingProfile : Profile
{
    public DefectRecordMappingProfile()
    {
        CreateMap<DefectRecord, DefectRecordDto>()
            .ForMember(dest => dest.PropertyUnitName, opt => opt.MapFrom(src => src.PropertyUnit != null ? src.PropertyUnit.UnitNumber : string.Empty));
        CreateMap<CreateDefectRecordDto, DefectRecord>();
        CreateMap<UpdateDefectRecordDto, DefectRecord>();
    }
}
