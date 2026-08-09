using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ContractorLedgers;

public record CreateContractorLedgerCommand(CreateContractorLedgerDto Dto) : IRequest<Guid>;

public class CreateContractorLedgerCommandHandler : IRequestHandler<CreateContractorLedgerCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateContractorLedgerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateContractorLedgerCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ContractorLedger>(request.Dto);

        var previousEntry = await _unitOfWork.Repository<ContractorLedger>().Query()
            .Where(l => l.ContractorId == entity.ContractorId)
            .OrderByDescending(l => l.TransactionDate)
            .ThenByDescending(l => l.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
        var previousBalance = previousEntry?.Balance ?? 0m;
        entity.Balance = previousBalance + entity.CreditAmount - entity.DebitAmount;

        await _unitOfWork.Repository<ContractorLedger>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
