using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DrawingApprovals;

public record GetDrawingApprovalByIdQuery(Guid Id) : IRequest<DrawingApprovalDto?>;

public class GetDrawingApprovalByIdQueryHandler : IRequestHandler<GetDrawingApprovalByIdQuery, DrawingApprovalDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDrawingApprovalByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DrawingApprovalDto?> Handle(GetDrawingApprovalByIdQuery request, CancellationToken cancellationToken)
    {
        var approval = await _unitOfWork.Repository<DrawingApproval>().Query()
            .Include(x => x.Drawing)
            .Include(x => x.DrawingRevision)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return approval is null ? null : _mapper.Map<DrawingApprovalDto>(approval);
    }
}
