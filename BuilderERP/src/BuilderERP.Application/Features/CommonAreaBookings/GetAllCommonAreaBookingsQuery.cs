using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CommonAreaBookings;

public record GetAllCommonAreaBookingsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<CommonAreaBookingDto>>;

public class GetAllCommonAreaBookingsQueryHandler : IRequestHandler<GetAllCommonAreaBookingsQuery, PagedResult<CommonAreaBookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCommonAreaBookingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CommonAreaBookingDto>> Handle(GetAllCommonAreaBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<CommonAreaBooking>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.BookingNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IReadOnlyList<CommonAreaBookingDto>>(items);
        return new PagedResult<CommonAreaBookingDto>(mapped, totalCount, page, pageSize);
    }
}
