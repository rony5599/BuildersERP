using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SecurityDeposits;

public record CreateSecurityDepositCommand(CreateSecurityDepositDto Dto) : IRequest<long>;

public class CreateSecurityDepositCommandHandler : IRequestHandler<CreateSecurityDepositCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSecurityDepositCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateSecurityDepositCommand request, CancellationToken cancellationToken)
    {
        var deposit = _mapper.Map<SecurityDeposit>(request.Dto);
        await _unitOfWork.Repository<SecurityDeposit>().AddAsync(deposit);
        await _unitOfWork.SaveChangesAsync();
        return deposit.Id;
    }
}
