using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record GetAllEngineerWorkOrdersQuery(
    int Page = 1,
    int PageSize = 25,
    string? WorkOrderNo = null,
    long? ProjectId = null,
    EngineerWorkOrderStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<EngineerWorkOrderDto>>;

public class GetAllEngineerWorkOrdersQueryHandler : IRequestHandler<GetAllEngineerWorkOrdersQuery, PagedResult<EngineerWorkOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEngineerWorkOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<EngineerWorkOrderDto>> Handle(GetAllEngineerWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<EngineerWorkOrder>().Query()
            .Include(o => o.Supplier)
            .Include(o => o.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(o => o.Details)
            .Where(o => o.IsLatestRevision)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.WorkOrderNo))
        {
            var term = request.WorkOrderNo.Trim();
            query = query.Where(o => o.WorkOrderNo.Contains(term));
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(o => o.EngineerWorkOrderRequisition.ProjectId == request.ProjectId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<EngineerWorkOrderDto>>(orders);
        return new PagedResult<EngineerWorkOrderDto>(items, totalCount, page, pageSize);
    }
}
