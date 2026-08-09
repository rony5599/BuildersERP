using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DrawingApprovals;

public record GetAllDrawingApprovalsQuery(Guid? DrawingId = null) : IRequest<IReadOnlyList<DrawingApprovalDto>>;

public class GetAllDrawingApprovalsQueryHandler : IRequestHandler<GetAllDrawingApprovalsQuery, IReadOnlyList<DrawingApprovalDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDrawingApprovalsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DrawingApprovalDto>> Handle(GetAllDrawingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DrawingApproval>().Query()
            .Include(x => x.Drawing)
            .Include(x => x.DrawingRevision)
            .AsQueryable();

        if (request.DrawingId.HasValue)
        {
            query = query.Where(x => x.DrawingId == request.DrawingId.Value);
        }

        var approvals = await query
            .OrderByDescending(x => x.RequestedDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<DrawingApprovalDto>>(approvals);
    }
}
