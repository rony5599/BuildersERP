using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockAdjustments;

public record GetAllStockAdjustmentsQuery : IRequest<IReadOnlyList<StockAdjustmentDto>>;

public class GetAllStockAdjustmentsQueryHandler : IRequestHandler<GetAllStockAdjustmentsQuery, IReadOnlyList<StockAdjustmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockAdjustmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<StockAdjustmentDto>> Handle(GetAllStockAdjustmentsQuery request, CancellationToken cancellationToken)
    {
        var adjustments = await _unitOfWork.Repository<StockAdjustment>().Query()
            .Include(a => a.Material)
            .Include(a => a.Warehouse)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StockAdjustmentDto>>(adjustments);
    }
}
