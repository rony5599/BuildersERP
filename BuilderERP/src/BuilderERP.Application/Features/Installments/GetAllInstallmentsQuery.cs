using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Installments;

public record GetAllInstallmentsQuery(
    long? ProjectId = null,
    long? PropertyUnitId = null,
    long? CustomerId = null,
    int Page = 1,
    int PageSize = 25) : IRequest<PagedResult<InstallmentDto>>;

public class GetAllInstallmentsQueryHandler : IRequestHandler<GetAllInstallmentsQuery, PagedResult<InstallmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInstallmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<InstallmentDto>> Handle(GetAllInstallmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Installment>().Query()
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.Customer)
            .Include(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.PropertyUnit).ThenInclude(u => u!.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(b => b.Project)
            .AsQueryable();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(i => i.InstallmentPlan.SaleAgreement.Booking.CustomerId == request.CustomerId.Value);
        }

        if (request.PropertyUnitId.HasValue)
        {
            query = query.Where(i => i.InstallmentPlan.SaleAgreement.Booking.PropertyUnitId == request.PropertyUnitId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(i => i.InstallmentPlan.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var installments = await query
            .OrderByDescending(i => i.DueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<InstallmentDto>>(installments);
        return new PagedResult<InstallmentDto>(items, totalCount, page, pageSize);
    }
}
