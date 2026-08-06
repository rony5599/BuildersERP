using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockTransfers;

public record GetStockTransferByIdQuery(Guid Id) : IRequest<StockTransferDto?>;

public class GetStockTransferByIdQueryHandler : IRequestHandler<GetStockTransferByIdQuery, StockTransferDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStockTransferByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StockTransferDto?> Handle(GetStockTransferByIdQuery request, CancellationToken cancellationToken)
    {
        var transfer = await _unitOfWork.Repository<StockTransfer>().GetByIdAsync(request.Id);
        return transfer is null ? null : _mapper.Map<StockTransferDto>(transfer);
    }
}
