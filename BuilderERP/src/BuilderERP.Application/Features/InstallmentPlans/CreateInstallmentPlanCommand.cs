using AutoMapper;
using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.InstallmentPlans;

public record CreateInstallmentPlanCommand(CreateInstallmentPlanDto Dto) : IRequest<long>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["Installments", "CollectionForecast"];
}

public class CreateInstallmentPlanCommandHandler : IRequestHandler<CreateInstallmentPlanCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInstallmentPlanCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateInstallmentPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = _mapper.Map<InstallmentPlan>(request.Dto);
        await _unitOfWork.Repository<InstallmentPlan>().AddAsync(plan);

        await GenerateScheduleAsync(_unitOfWork, plan);

        await _unitOfWork.SaveChangesAsync();

        return plan.Id;
    }

    internal static async Task GenerateScheduleAsync(IUnitOfWork unitOfWork, InstallmentPlan plan)
    {
        var interestMultiplier = 1 + (plan.InterestRatePercent ?? 0) / 100m;
        var totalWithInterest = plan.TotalAmount * interestMultiplier;

        var baseAmount = Math.Round(totalWithInterest / plan.NumberOfInstallments, 2, MidpointRounding.AwayFromZero);
        var allocated = baseAmount * (plan.NumberOfInstallments - 1);
        var lastAmount = Math.Round(totalWithInterest - allocated, 2, MidpointRounding.AwayFromZero);

        var repository = unitOfWork.Repository<Installment>();

        for (var i = 1; i <= plan.NumberOfInstallments; i++)
        {
            var installment = new Installment
            {
                InstallmentPlan = plan,
                InstallmentNumber = i,
                DueDate = plan.StartDate.AddMonths(i - 1),
                DueAmount = i == plan.NumberOfInstallments ? lastAmount : baseAmount
            };

            await repository.AddAsync(installment);
        }
    }
}
