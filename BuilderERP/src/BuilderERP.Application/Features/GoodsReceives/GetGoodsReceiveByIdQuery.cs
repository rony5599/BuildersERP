using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.GoodsReceives;

public record GetGoodsReceiveByIdQuery(Guid Id) : IRequest<GoodsReceiveDto?>;

public class GetGoodsReceiveByIdQueryHandler : IRequestHandler<GetGoodsReceiveByIdQuery, GoodsReceiveDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetGoodsReceiveByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GoodsReceiveDto?> Handle(GetGoodsReceiveByIdQuery request, CancellationToken cancellationToken)
    {
        var receive = await _unitOfWork.Repository<GoodsReceive>().GetByIdAsync(request.Id);
        return receive is null ? null : _mapper.Map<GoodsReceiveDto>(receive);
    }
}
