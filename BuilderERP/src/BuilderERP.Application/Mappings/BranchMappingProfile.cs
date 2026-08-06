using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class BranchMappingProfile : Profile
{
    public BranchMappingProfile()
    {
        CreateMap<Branch, BranchDto>();
        CreateMap<CreateBranchDto, Branch>();
        CreateMap<UpdateBranchDto, Branch>();
    }
}
