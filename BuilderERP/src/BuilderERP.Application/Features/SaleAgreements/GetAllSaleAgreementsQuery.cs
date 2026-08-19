using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SaleAgreements;

public record GetAllSaleAgreementsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<SaleAgreementDto>>;

public class GetAllSaleAgreementsQueryHandler : IRequestHandler<GetAllSaleAgreementsQuery, PagedResult<SaleAgreementDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSaleAgreementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SaleAgreementDto>> Handle(GetAllSaleAgreementsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SaleAgreement>().Query()
            .Include(a => a.Booking)
            .ThenInclude(b => b.PropertyUnit);

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var agreements = await query
            .OrderByDescending(a => a.AgreementDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SaleAgreementDto>>(agreements);
        return new PagedResult<SaleAgreementDto>(items, totalCount, page, pageSize);
    }
}
