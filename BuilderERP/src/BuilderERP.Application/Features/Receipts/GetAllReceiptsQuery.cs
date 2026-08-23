using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Receipts;

public record GetAllReceiptsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<ReceiptDto>>;

public class GetAllReceiptsQueryHandler : IRequestHandler<GetAllReceiptsQuery, PagedResult<ReceiptDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllReceiptsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ReceiptDto>> Handle(GetAllReceiptsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Receipt>().Query()
            .Include(r => r.Installment).ThenInclude(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.PropertyUnit).ThenInclude(u => u!.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(bd => bd.Project);

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var receipts = await query
            .OrderByDescending(r => r.PaymentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<ReceiptDto>>(receipts);
        return new PagedResult<ReceiptDto>(items, totalCount, page, pageSize);
    }
}
