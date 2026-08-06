using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Stocks;

public record GetAllStockQuery : IRequest<IReadOnlyList<StockDto>>;

public class GetAllStockQueryHandler : IRequestHandler<GetAllStockQuery, IReadOnlyList<StockDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<StockDto>> Handle(GetAllStockQuery request, CancellationToken cancellationToken)
    {
        var stocks = await _unitOfWork.Repository<Stock>().Query()
            .Include(s => s.Material)
            .Include(s => s.Warehouse)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StockDto>>(stocks);
    }
}
