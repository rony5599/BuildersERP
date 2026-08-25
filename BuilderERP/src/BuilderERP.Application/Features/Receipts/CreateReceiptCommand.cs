using AutoMapper;
using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Receipts;

public record CreateReceiptCommand(CreateReceiptDto Dto) : IRequest<long>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["Installments", "CollectionForecast"];
}

public class CreateReceiptCommandHandler : IRequestHandler<CreateReceiptCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateReceiptCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
    {
        var receipt = _mapper.Map<Receipt>(request.Dto);
        await _unitOfWork.Repository<Receipt>().AddAsync(receipt);

        await ReceiptInstallmentSync.ApplyAsync(_unitOfWork, receipt.InstallmentId, receipt.AmountPaid);

        await _unitOfWork.SaveChangesAsync();

        return receipt.Id;
    }
}

internal static class ReceiptInstallmentSync
{
    public static async Task ApplyAsync(IUnitOfWork unitOfWork, long installmentId, decimal amountPaid)
    {
        var repository = unitOfWork.Repository<Installment>();
        var installment = await repository.GetByIdAsync(installmentId);
        if (installment is null)
        {
            return;
        }

        installment.PaidAmount += amountPaid;

        var totalDue = installment.DueAmount + installment.PenaltyAmount;
        installment.Status = installment.PaidAmount >= totalDue
            ? InstallmentStatus.Paid
            : installment.PaidAmount > 0
                ? InstallmentStatus.PartiallyPaid
                : InstallmentStatus.Pending;

        repository.Update(installment);
    }
}
