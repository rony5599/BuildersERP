using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CommonAreaBookings;

public record GetAllCommonAreaBookingsQuery : IRequest<IReadOnlyList<CommonAreaBookingDto>>;

public class GetAllCommonAreaBookingsQueryHandler : IRequestHandler<GetAllCommonAreaBookingsQuery, IReadOnlyList<CommonAreaBookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCommonAreaBookingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CommonAreaBookingDto>> Handle(GetAllCommonAreaBookingsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<CommonAreaBooking>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.BookingNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CommonAreaBookingDto>>(items);
    }
}
