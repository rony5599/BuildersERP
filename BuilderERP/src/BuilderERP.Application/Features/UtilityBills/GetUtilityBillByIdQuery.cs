using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.UtilityBills;

public record GetUtilityBillByIdQuery(long Id) : IRequest<UtilityBillDto?>;

public class GetUtilityBillByIdQueryHandler : IRequestHandler<GetUtilityBillByIdQuery, UtilityBillDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUtilityBillByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UtilityBillDto?> Handle(GetUtilityBillByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<UtilityBill>().Query()
            .Include(x => x.PropertyUnit)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<UtilityBillDto>(item);
    }
}
