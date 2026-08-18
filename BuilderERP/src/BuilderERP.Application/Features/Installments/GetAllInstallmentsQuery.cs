using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Installments;

public record GetAllInstallmentsQuery(
    Guid? ProjectId = null,
    Guid? PropertyUnitId = null,
    Guid? CustomerId = null) : IRequest<IReadOnlyList<InstallmentDto>>;

public class GetAllInstallmentsQueryHandler : IRequestHandler<GetAllInstallmentsQuery, IReadOnlyList<InstallmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInstallmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InstallmentDto>> Handle(GetAllInstallmentsQuery request, CancellationToken cancellationToken)
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

        var installments = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<InstallmentDto>>(installments);
    }
}
