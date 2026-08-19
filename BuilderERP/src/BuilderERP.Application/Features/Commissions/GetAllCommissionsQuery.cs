using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Commissions;

public record GetAllCommissionsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<CommissionDto>>;

public class GetAllCommissionsQueryHandler : IRequestHandler<GetAllCommissionsQuery, PagedResult<CommissionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCommissionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CommissionDto>> Handle(GetAllCommissionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Commission>().Query()
            .Include(c => c.Broker)
            .Include(c => c.Booking)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var commissions = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<CommissionDto>>(commissions);
        return new PagedResult<CommissionDto>(items, totalCount, page, pageSize);
    }
}
