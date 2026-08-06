using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.InstallmentPlans;

public record GetAllInstallmentPlansQuery : IRequest<IReadOnlyList<InstallmentPlanDto>>;

public class GetAllInstallmentPlansQueryHandler : IRequestHandler<GetAllInstallmentPlansQuery, IReadOnlyList<InstallmentPlanDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInstallmentPlansQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InstallmentPlanDto>> Handle(GetAllInstallmentPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _unitOfWork.Repository<InstallmentPlan>().Query()
            .Include(p => p.SaleAgreement)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<InstallmentPlanDto>>(plans);
    }
}
