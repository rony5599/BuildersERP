using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Rfqs;

public record GetAllRfqsQuery(
    int Page = 1,
    int PageSize = 25,
    string? RfqNumber = null,
    long? ProjectId = null,
    RfqStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<RfqDto>>;

public class GetAllRfqsQueryHandler : IRequestHandler<GetAllRfqsQuery, PagedResult<RfqDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRfqsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<RfqDto>> Handle(GetAllRfqsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Rfq>().Query()
            .Include(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(r => r.RfqVendors).ThenInclude(v => v.Supplier)
            .Include(r => r.Details)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.RfqNumber))
        {
            var term = request.RfqNumber.Trim();
            query = query.Where(r => r.RfqNumber.Contains(term));
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(r => r.PurchaseRequisition.ProjectId == request.ProjectId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(r => r.IssueDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(r => r.IssueDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var rfqs = await query
            .OrderByDescending(r => r.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<RfqDto>>(rfqs);
        return new PagedResult<RfqDto>(items, totalCount, page, pageSize);
    }
}
