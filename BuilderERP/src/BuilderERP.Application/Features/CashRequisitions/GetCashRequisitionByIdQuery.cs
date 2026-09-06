using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashRequisitions;

public record GetCashRequisitionByIdQuery(long Id) : IRequest<CashRequisitionDto?>;

public class GetCashRequisitionByIdQueryHandler : IRequestHandler<GetCashRequisitionByIdQuery, CashRequisitionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCashRequisitionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CashRequisitionDto?> Handle(GetCashRequisitionByIdQuery request, CancellationToken cancellationToken)
    {
        var requisition = await _unitOfWork.Repository<CashRequisition>().Query()
            .Include(r => r.RequesterEmployee)
            .Include(r => r.Department)
            .Include(r => r.Project)
            .Include(r => r.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        return requisition is null ? null : _mapper.Map<CashRequisitionDto>(requisition);
    }
}
