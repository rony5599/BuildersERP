using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record GetAllPurchaseRequisitionsQuery : IRequest<IReadOnlyList<PurchaseRequisitionDto>>;

public class GetAllPurchaseRequisitionsQueryHandler : IRequestHandler<GetAllPurchaseRequisitionsQuery, IReadOnlyList<PurchaseRequisitionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPurchaseRequisitionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PurchaseRequisitionDto>> Handle(GetAllPurchaseRequisitionsQuery request, CancellationToken cancellationToken)
    {
        var requisitions = await _unitOfWork.Repository<PurchaseRequisition>().Query()
            .Include(r => r.Department)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PurchaseRequisitionDto>>(requisitions);
    }
}
