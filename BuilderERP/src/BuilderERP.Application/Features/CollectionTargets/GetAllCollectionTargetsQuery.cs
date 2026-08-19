using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CollectionTargets;

public record GetAllCollectionTargetsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<CollectionTargetDto>>;

public class GetAllCollectionTargetsQueryHandler : IRequestHandler<GetAllCollectionTargetsQuery, PagedResult<CollectionTargetDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCollectionTargetsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CollectionTargetDto>> Handle(GetAllCollectionTargetsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var query = _unitOfWork.Repository<CollectionTarget>().Query()
            .Include(t => t.CollectionOfficer)
            .OrderByDescending(t => t.Year).ThenByDescending(t => t.Month);

        var totalCount = await query.CountAsync(cancellationToken);
        var targets = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var receipts = await _unitOfWork.Repository<Receipt>().Query()
            .Include(r => r.Installment).ThenInclude(i => i.InstallmentPlan).ThenInclude(p => p.SaleAgreement).ThenInclude(a => a.Booking)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<CollectionTargetDto>>(targets);

        foreach (var dto in dtos)
        {
            var periodStart = new DateTime(dto.Year, dto.Month, 1);
            var periodEnd = periodStart.AddMonths(1);

            dto.ActualCollected = receipts
                .Where(r => r.PaymentDate >= periodStart && r.PaymentDate < periodEnd
                    && r.Installment?.InstallmentPlan?.SaleAgreement?.Booking?.CollectionOfficerId == dto.CollectionOfficerId)
                .Sum(r => r.AmountPaid);

            dto.AchievementPercent = dto.TargetAmount == 0 ? 0 : Math.Round(dto.ActualCollected * 100m / dto.TargetAmount, 1);
        }

        return new PagedResult<CollectionTargetDto>(dtos, totalCount, page, pageSize);
    }
}
