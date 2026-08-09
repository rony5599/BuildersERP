using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ContractorLedgers;

public record GetAllContractorLedgersQuery(Guid? ContractorId = null) : IRequest<IReadOnlyList<ContractorLedgerDto>>;

public class GetAllContractorLedgersQueryHandler : IRequestHandler<GetAllContractorLedgersQuery, IReadOnlyList<ContractorLedgerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllContractorLedgersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ContractorLedgerDto>> Handle(GetAllContractorLedgersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<ContractorLedger>().Query()
            .Include(x => x.Contractor)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var ledgers = await query
            .OrderBy(x => x.TransactionDate)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<ContractorLedgerDto>>(ledgers);
    }
}
