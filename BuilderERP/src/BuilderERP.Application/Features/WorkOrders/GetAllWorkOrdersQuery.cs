using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.WorkOrders;

public record GetAllWorkOrdersQuery(Guid? ContractorId = null, Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<WorkOrderDto>>;

public class GetAllWorkOrdersQueryHandler : IRequestHandler<GetAllWorkOrdersQuery, PagedResult<WorkOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWorkOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<WorkOrderDto>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<WorkOrder>().Query()
            .Include(x => x.Contractor)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var workOrders = await query
            .OrderByDescending(x => x.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<WorkOrderDto>>(workOrders);
        return new PagedResult<WorkOrderDto>(items, totalCount, page, pageSize);
    }
}
