using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class CommonAreaBookingMappingProfile : Profile
{
    public CommonAreaBookingMappingProfile()
    {
        CreateMap<CommonAreaBooking, CommonAreaBookingDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty));
        CreateMap<CreateCommonAreaBookingDto, CommonAreaBooking>();
        CreateMap<UpdateCommonAreaBookingDto, CommonAreaBooking>();
    }
}
