using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ContractorLedgers;

public record GetContractorLedgerByIdQuery(long Id) : IRequest<ContractorLedgerDto?>;

public class GetContractorLedgerByIdQueryHandler : IRequestHandler<GetContractorLedgerByIdQuery, ContractorLedgerDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContractorLedgerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContractorLedgerDto?> Handle(GetContractorLedgerByIdQuery request, CancellationToken cancellationToken)
    {
        var ledger = await _unitOfWork.Repository<ContractorLedger>().GetByIdAsync(request.Id);
        return ledger is null ? null : _mapper.Map<ContractorLedgerDto>(ledger);
    }
}
