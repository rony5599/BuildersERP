using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DrawingApprovals;

public record GetAllDrawingApprovalsQuery(Guid? DrawingId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<DrawingApprovalDto>>;

public class GetAllDrawingApprovalsQueryHandler : IRequestHandler<GetAllDrawingApprovalsQuery, PagedResult<DrawingApprovalDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDrawingApprovalsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DrawingApprovalDto>> Handle(GetAllDrawingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DrawingApproval>().Query()
            .Include(x => x.Drawing)
            .Include(x => x.DrawingRevision)
            .AsQueryable();

        if (request.DrawingId.HasValue)
        {
            query = query.Where(x => x.DrawingId == request.DrawingId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var approvals = await query
            .OrderByDescending(x => x.RequestedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<DrawingApprovalDto>>(approvals);
        return new PagedResult<DrawingApprovalDto>(items, totalCount, page, pageSize);
    }
}
