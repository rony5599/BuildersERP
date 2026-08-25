using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RateContracts;

public record CreateRateContractCommand(CreateRateContractDto Dto) : IRequest<long>;

public class CreateRateContractCommandHandler : IRequestHandler<CreateRateContractCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRateContractCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateRateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = _mapper.Map<RateContract>(request.Dto);
        await _unitOfWork.Repository<RateContract>().AddAsync(contract);
        await _unitOfWork.SaveChangesAsync();
        return contract.Id;
    }
}
