using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.InstallmentPlans;

public record GetAllInstallmentPlansQuery(
    long? ProjectId = null,
    long? PropertyUnitId = null,
    long? CustomerId = null,
    int Page = 1,
    int PageSize = 25) : IRequest<PagedResult<InstallmentPlanDto>>;

public class GetAllInstallmentPlansQueryHandler : IRequestHandler<GetAllInstallmentPlansQuery, PagedResult<InstallmentPlanDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInstallmentPlansQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<InstallmentPlanDto>> Handle(GetAllInstallmentPlansQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<InstallmentPlan>().Query()
            .Include(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.Customer)
            .Include(p => p.SaleAgreement).ThenInclude(a => a.Booking).ThenInclude(b => b.PropertyUnit).ThenInclude(u => u!.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(b => b.Project)
            .AsQueryable();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(p => p.SaleAgreement.Booking.CustomerId == request.CustomerId.Value);
        }

        if (request.PropertyUnitId.HasValue)
        {
            query = query.Where(p => p.SaleAgreement.Booking.PropertyUnitId == request.PropertyUnitId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(p => p.SaleAgreement.Booking.PropertyUnit.Floor.Tower.Building.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var plans = await query
            .OrderByDescending(p => p.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<InstallmentPlanDto>>(plans);
        return new PagedResult<InstallmentPlanDto>(items, totalCount, page, pageSize);
    }
}
