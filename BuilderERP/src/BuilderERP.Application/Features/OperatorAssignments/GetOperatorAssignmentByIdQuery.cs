using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.OperatorAssignments;

public record GetOperatorAssignmentByIdQuery(long Id) : IRequest<OperatorAssignmentDto?>;

public class GetOperatorAssignmentByIdQueryHandler : IRequestHandler<GetOperatorAssignmentByIdQuery, OperatorAssignmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOperatorAssignmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperatorAssignmentDto?> Handle(GetOperatorAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Repository<OperatorAssignment>().GetByIdAsync(request.Id);
        return assignment is null ? null : _mapper.Map<OperatorAssignmentDto>(assignment);
    }
}
