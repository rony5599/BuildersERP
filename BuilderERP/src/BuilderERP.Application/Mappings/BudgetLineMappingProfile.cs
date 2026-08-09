using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class BudgetLineMappingProfile : Profile
{
    public BudgetLineMappingProfile()
    {
        CreateMap<BudgetLine, BudgetLineDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateBudgetLineDto, BudgetLine>();
        CreateMap<UpdateBudgetLineDto, BudgetLine>();
    }
}
