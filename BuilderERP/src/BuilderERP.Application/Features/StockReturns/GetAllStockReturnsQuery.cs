using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockReturns;

public record GetAllStockReturnsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<StockReturnDto>>;

public class GetAllStockReturnsQueryHandler : IRequestHandler<GetAllStockReturnsQuery, IReadOnlyList<StockReturnDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockReturnsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<StockReturnDto>> Handle(GetAllStockReturnsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<StockReturn>().Query()
            .Include(r => r.Material)
            .Include(r => r.Warehouse).ThenInclude(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(r => r.Warehouse.ProjectId == request.ProjectId.Value);
        }

        var returns = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StockReturnDto>>(returns);
    }
}
