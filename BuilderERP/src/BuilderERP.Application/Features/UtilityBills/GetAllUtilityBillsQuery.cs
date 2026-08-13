using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.UtilityBills;

public record GetAllUtilityBillsQuery : IRequest<IReadOnlyList<UtilityBillDto>>;

public class GetAllUtilityBillsQueryHandler : IRequestHandler<GetAllUtilityBillsQuery, IReadOnlyList<UtilityBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUtilityBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<UtilityBillDto>> Handle(GetAllUtilityBillsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<UtilityBill>().Query()
            .Include(x => x.PropertyUnit)
            .OrderBy(x => x.BillNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<UtilityBillDto>>(items);
    }
}
