using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RunningBills;

public record CertifyRunningBillCommand(CertifyRunningBillDto Dto) : IRequest<bool>;

public class CertifyRunningBillCommandHandler : IRequestHandler<CertifyRunningBillCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CertifyRunningBillCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CertifyRunningBillCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<RunningBill>();
        var bill = await repository.GetByIdAsync(request.Dto.Id);
        if (bill is null)
        {
            return false;
        }

        bill.CertificateNumber = request.Dto.CertificateNumber;
        bill.CertifiedBy = request.Dto.CertifiedBy;
        bill.CertificationDate = DateTime.UtcNow;
        if (bill.Status == RunningBillStatus.Submitted)
        {
            bill.Status = RunningBillStatus.Approved;
        }

        repository.Update(bill);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
