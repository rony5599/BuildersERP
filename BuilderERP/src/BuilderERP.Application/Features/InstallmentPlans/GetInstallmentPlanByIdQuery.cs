using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.InstallmentPlans;

public record GetInstallmentPlanByIdQuery(long Id) : IRequest<InstallmentPlanDto?>;

public class GetInstallmentPlanByIdQueryHandler : IRequestHandler<GetInstallmentPlanByIdQuery, InstallmentPlanDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetInstallmentPlanByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InstallmentPlanDto?> Handle(GetInstallmentPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await _unitOfWork.Repository<InstallmentPlan>().GetByIdAsync(request.Id);
        return plan is null ? null : _mapper.Map<InstallmentPlanDto>(plan);
    }
}
