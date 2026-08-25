using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record GetEngineerWorkOrderByIdQuery(long Id) : IRequest<EngineerWorkOrderDto?>;

public class GetEngineerWorkOrderByIdQueryHandler : IRequestHandler<GetEngineerWorkOrderByIdQuery, EngineerWorkOrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEngineerWorkOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EngineerWorkOrderDto?> Handle(GetEngineerWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await _unitOfWork.Repository<EngineerWorkOrder>().Query()
            .Include(o => o.Supplier)
            .Include(o => o.EngineerWorkOrderRequisition)
            .Include(o => o.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        return workOrder is null ? null : _mapper.Map<EngineerWorkOrderDto>(workOrder);
    }
}
