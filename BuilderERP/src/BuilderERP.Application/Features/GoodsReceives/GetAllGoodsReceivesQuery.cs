using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.GoodsReceives;

public record GetAllGoodsReceivesQuery : IRequest<IReadOnlyList<GoodsReceiveDto>>;

public class GetAllGoodsReceivesQueryHandler : IRequestHandler<GetAllGoodsReceivesQuery, IReadOnlyList<GoodsReceiveDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllGoodsReceivesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<GoodsReceiveDto>> Handle(GetAllGoodsReceivesQuery request, CancellationToken cancellationToken)
    {
        var receives = await _unitOfWork.Repository<GoodsReceive>().Query()
            .Include(g => g.PurchaseOrder)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<GoodsReceiveDto>>(receives);
    }
}
