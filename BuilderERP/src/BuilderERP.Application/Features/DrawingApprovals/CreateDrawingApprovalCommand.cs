using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DrawingApprovals;

public record CreateDrawingApprovalCommand(CreateDrawingApprovalDto Dto) : IRequest<Guid>;

public class CreateDrawingApprovalCommandHandler : IRequestHandler<CreateDrawingApprovalCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDrawingApprovalCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateDrawingApprovalCommand request, CancellationToken cancellationToken)
    {
        var approval = _mapper.Map<DrawingApproval>(request.Dto);
        await _unitOfWork.Repository<DrawingApproval>().AddAsync(approval);
        await _unitOfWork.SaveChangesAsync();
        return approval.Id;
    }
}
