using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockAdjustments;

public record GetStockAdjustmentByIdQuery(long Id) : IRequest<StockAdjustmentDto?>;

public class GetStockAdjustmentByIdQueryHandler : IRequestHandler<GetStockAdjustmentByIdQuery, StockAdjustmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStockAdjustmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StockAdjustmentDto?> Handle(GetStockAdjustmentByIdQuery request, CancellationToken cancellationToken)
    {
        var adjustment = await _unitOfWork.Repository<StockAdjustment>().GetByIdAsync(request.Id);
        return adjustment is null ? null : _mapper.Map<StockAdjustmentDto>(adjustment);
    }
}
