using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Bookings;

public record GetAllBookingsQuery : IRequest<IReadOnlyList<BookingDto>>;

public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, IReadOnlyList<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBookingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BookingDto>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _unitOfWork.Repository<Booking>().Query()
            .Include(b => b.Customer)
            .Include(b => b.PropertyUnit)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BookingDto>>(bookings);
    }
}
