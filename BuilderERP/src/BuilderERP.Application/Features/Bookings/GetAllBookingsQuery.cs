using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Bookings;

public record GetAllBookingsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<BookingDto>>;

public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, PagedResult<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBookingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<BookingDto>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Booking>().Query()
            .Include(b => b.Customer)
            .Include(b => b.PropertyUnit).ThenInclude(u => u.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(bd => bd.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var bookings = await query
            .OrderByDescending(b => b.BookingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<BookingDto>>(bookings);
        return new PagedResult<BookingDto>(items, totalCount, page, pageSize);
    }
}
