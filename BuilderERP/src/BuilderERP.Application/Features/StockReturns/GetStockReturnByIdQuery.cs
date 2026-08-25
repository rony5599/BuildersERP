using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockReturns;

public record GetStockReturnByIdQuery(long Id) : IRequest<StockReturnDto?>;

public class GetStockReturnByIdQueryHandler : IRequestHandler<GetStockReturnByIdQuery, StockReturnDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStockReturnByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StockReturnDto?> Handle(GetStockReturnByIdQuery request, CancellationToken cancellationToken)
    {
        var stockReturn = await _unitOfWork.Repository<StockReturn>().GetByIdAsync(request.Id);
        return stockReturn is null ? null : _mapper.Map<StockReturnDto>(stockReturn);
    }
}
