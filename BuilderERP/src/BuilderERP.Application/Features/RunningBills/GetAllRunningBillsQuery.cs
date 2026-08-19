using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.RunningBills;

public record GetAllRunningBillsQuery(Guid? WorkOrderId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<RunningBillDto>>;

public class GetAllRunningBillsQueryHandler : IRequestHandler<GetAllRunningBillsQuery, PagedResult<RunningBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRunningBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<RunningBillDto>> Handle(GetAllRunningBillsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<RunningBill>().Query()
            .Include(x => x.WorkOrder)
            .AsQueryable();

        if (request.WorkOrderId.HasValue)
        {
            query = query.Where(x => x.WorkOrderId == request.WorkOrderId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var bills = await query
            .OrderByDescending(x => x.BillDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<RunningBillDto>>(bills);
        return new PagedResult<RunningBillDto>(items, totalCount, page, pageSize);
    }
}
