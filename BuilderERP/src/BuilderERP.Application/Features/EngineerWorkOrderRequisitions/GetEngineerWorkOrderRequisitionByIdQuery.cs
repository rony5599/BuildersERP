using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrderRequisitions;

public record GetEngineerWorkOrderRequisitionByIdQuery(Guid Id) : IRequest<EngineerWorkOrderRequisitionDto?>;

public class GetEngineerWorkOrderRequisitionByIdQueryHandler : IRequestHandler<GetEngineerWorkOrderRequisitionByIdQuery, EngineerWorkOrderRequisitionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEngineerWorkOrderRequisitionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EngineerWorkOrderRequisitionDto?> Handle(GetEngineerWorkOrderRequisitionByIdQuery request, CancellationToken cancellationToken)
    {
        var requisition = await _unitOfWork.Repository<EngineerWorkOrderRequisition>().Query()
            .Include(r => r.Project)
            .Include(r => r.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        return requisition is null ? null : _mapper.Map<EngineerWorkOrderRequisitionDto>(requisition);
    }
}
