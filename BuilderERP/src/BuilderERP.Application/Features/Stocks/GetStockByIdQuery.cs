using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Stocks;

public record GetStockByIdQuery(Guid Id) : IRequest<StockDto?>;

public class GetStockByIdQueryHandler : IRequestHandler<GetStockByIdQuery, StockDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStockByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StockDto?> Handle(GetStockByIdQuery request, CancellationToken cancellationToken)
    {
        var stock = await _unitOfWork.Repository<Stock>().Query()
            .Include(s => s.Material)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        return stock is null ? null : _mapper.Map<StockDto>(stock);
    }
}
