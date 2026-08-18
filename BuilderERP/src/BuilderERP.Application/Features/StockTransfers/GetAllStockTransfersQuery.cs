using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockTransfers;

public record GetAllStockTransfersQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<StockTransferDto>>;

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
        var query = _unitOfWork.Repository<StockTransfer>().Query()
            .Include(t => t.Material)
            .Include(t => t.FromWarehouse).ThenInclude(w => w.Project)
            .Include(t => t.ToWarehouse).ThenInclude(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(t => t.FromWarehouse.ProjectId == request.ProjectId.Value || t.ToWarehouse.ProjectId == request.ProjectId.Value);
        }

        var transfers = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StockTransferDto>>(transfers);
    }
}
