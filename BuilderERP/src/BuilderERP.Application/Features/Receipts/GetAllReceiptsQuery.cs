using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Receipts;

public record GetAllReceiptsQuery : IRequest<IReadOnlyList<ReceiptDto>>;

public class GetAllReceiptsQueryHandler : IRequestHandler<GetAllReceiptsQuery, IReadOnlyList<ReceiptDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllReceiptsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReceiptDto>> Handle(GetAllReceiptsQuery request, CancellationToken cancellationToken)
    {
        var receipts = await _unitOfWork.Repository<Receipt>().Query()
            .Include(r => r.Installment)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ReceiptDto>>(receipts);
    }
}
