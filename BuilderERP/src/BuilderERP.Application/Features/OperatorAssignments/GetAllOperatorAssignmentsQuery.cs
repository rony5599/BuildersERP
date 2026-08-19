using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.OperatorAssignments;

public record GetAllOperatorAssignmentsQuery(Guid? EquipmentId = null, Guid? WorkerId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<OperatorAssignmentDto>>;

public class GetAllOperatorAssignmentsQueryHandler : IRequestHandler<GetAllOperatorAssignmentsQuery, PagedResult<OperatorAssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOperatorAssignmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<OperatorAssignmentDto>> Handle(GetAllOperatorAssignmentsQuery request, CancellationToken cancellationToken)
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

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var assignments = await query
            .OrderByDescending(x => x.AssignmentStartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<OperatorAssignmentDto>>(assignments);
        return new PagedResult<OperatorAssignmentDto>(items, totalCount, page, pageSize);
    }
}
