using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class TestReportMappingProfile : Profile
{
    public TestReportMappingProfile()
    {
        CreateMap<TestReport, TestReportDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.MaterialName, opt => opt.MapFrom(src => src.Material != null ? src.Material.Name : null));
        CreateMap<CreateTestReportDto, TestReport>();
        CreateMap<UpdateTestReportDto, TestReport>();
    }
}
