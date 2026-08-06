using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockTransfers;

public record GetAllStockTransfersQuery : IRequest<IReadOnlyList<StockTransferDto>>;

public class GetAllStockTransfersQueryHandler : IRequestHandler<GetAllStockTransfersQuery, IReadOnlyList<StockTransferDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockTransfersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<StockTransferDto>> Handle(GetAllStockTransfersQuery request, CancellationToken cancellationToken)
    {
        var transfers = await _unitOfWork.Repository<StockTransfer>().Query()
            .Include(t => t.Material)
            .Include(t => t.FromWarehouse)
            .Include(t => t.ToWarehouse)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StockTransferDto>>(transfers);
    }
}
