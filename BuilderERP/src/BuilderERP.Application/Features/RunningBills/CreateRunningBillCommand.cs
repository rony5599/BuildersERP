using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RunningBills;

public record CreateRunningBillCommand(CreateRunningBillDto Dto) : IRequest<long>;

public class CreateRunningBillCommandHandler : IRequestHandler<CreateRunningBillCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRunningBillCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateRunningBillCommand request, CancellationToken cancellationToken)
    {
        var bill = _mapper.Map<RunningBill>(request.Dto);
        bill.NetPayableAmount = bill.WorkDoneAmount - bill.PreviousBillAmount - bill.DeductionAmount;
        await _unitOfWork.Repository<RunningBill>().AddAsync(bill);
        await _unitOfWork.SaveChangesAsync();
        return bill.Id;
    }
}
