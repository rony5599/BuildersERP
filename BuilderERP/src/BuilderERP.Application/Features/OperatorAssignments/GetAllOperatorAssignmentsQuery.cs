using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.OperatorAssignments;

public record GetAllOperatorAssignmentsQuery(Guid? EquipmentId = null, Guid? WorkerId = null) : IRequest<IReadOnlyList<OperatorAssignmentDto>>;

public class GetAllOperatorAssignmentsQueryHandler : IRequestHandler<GetAllOperatorAssignmentsQuery, IReadOnlyList<OperatorAssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOperatorAssignmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<OperatorAssignmentDto>> Handle(GetAllOperatorAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<OperatorAssignment>().Query()
            .Include(x => x.Equipment)
            .Include(x => x.Worker)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(x => x.EquipmentId == request.EquipmentId.Value);
        }

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var assignments = await query
            .OrderByDescending(x => x.AssignmentStartDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<OperatorAssignmentDto>>(assignments);
    }
}
