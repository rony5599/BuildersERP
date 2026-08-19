using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ServiceTickets;

public record GetAllServiceTicketsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<ServiceTicketDto>>;

public class GetAllServiceTicketsQueryHandler : IRequestHandler<GetAllServiceTicketsQuery, PagedResult<ServiceTicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllServiceTicketsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ServiceTicketDto>> Handle(GetAllServiceTicketsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<ServiceTicket>().Query()
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.TicketNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<ServiceTicketDto>>(results);
        return new PagedResult<ServiceTicketDto>(items, totalCount, page, pageSize);
    }
}
