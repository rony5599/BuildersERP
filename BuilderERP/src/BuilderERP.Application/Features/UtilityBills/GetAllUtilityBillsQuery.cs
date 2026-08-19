using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.UtilityBills;

public record GetAllUtilityBillsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<UtilityBillDto>>;

public class GetAllUtilityBillsQueryHandler : IRequestHandler<GetAllUtilityBillsQuery, PagedResult<UtilityBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUtilityBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<UtilityBillDto>> Handle(GetAllUtilityBillsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UtilityBill>().Query()
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.BillNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<UtilityBillDto>>(results);
        return new PagedResult<UtilityBillDto>(items, totalCount, page, pageSize);
    }
}
