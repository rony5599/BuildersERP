using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.RunningBills;

public record GetAllRunningBillsQuery(Guid? WorkOrderId = null) : IRequest<IReadOnlyList<RunningBillDto>>;

public class GetAllRunningBillsQueryHandler : IRequestHandler<GetAllRunningBillsQuery, IReadOnlyList<RunningBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRunningBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RunningBillDto>> Handle(GetAllRunningBillsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<RunningBill>().Query()
            .Include(x => x.WorkOrder)
            .AsQueryable();

        if (request.WorkOrderId.HasValue)
        {
            query = query.Where(x => x.WorkOrderId == request.WorkOrderId.Value);
        }

        var bills = await query
            .OrderByDescending(x => x.BillDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<RunningBillDto>>(bills);
    }
}
