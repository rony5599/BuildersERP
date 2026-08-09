using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.OperatorAssignments;

public record CreateOperatorAssignmentCommand(CreateOperatorAssignmentDto Dto) : IRequest<Guid>;

public class CreateOperatorAssignmentCommandHandler : IRequestHandler<CreateOperatorAssignmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOperatorAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateOperatorAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = _mapper.Map<OperatorAssignment>(request.Dto);
        await _unitOfWork.Repository<OperatorAssignment>().AddAsync(assignment);
        await _unitOfWork.SaveChangesAsync();
        return assignment.Id;
    }
}
