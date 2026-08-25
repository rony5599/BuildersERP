using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RunningBills;

public record GetRunningBillByIdQuery(long Id) : IRequest<RunningBillDto?>;

public class GetRunningBillByIdQueryHandler : IRequestHandler<GetRunningBillByIdQuery, RunningBillDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRunningBillByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RunningBillDto?> Handle(GetRunningBillByIdQuery request, CancellationToken cancellationToken)
    {
        var bill = await _unitOfWork.Repository<RunningBill>().GetByIdAsync(request.Id);
        return bill is null ? null : _mapper.Map<RunningBillDto>(bill);
    }
}
