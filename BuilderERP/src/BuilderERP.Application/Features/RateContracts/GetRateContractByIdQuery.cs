using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RateContracts;

public record GetRateContractByIdQuery(long Id) : IRequest<RateContractDto?>;

public class GetRateContractByIdQueryHandler : IRequestHandler<GetRateContractByIdQuery, RateContractDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRateContractByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RateContractDto?> Handle(GetRateContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await _unitOfWork.Repository<RateContract>().GetByIdAsync(request.Id);
        return contract is null ? null : _mapper.Map<RateContractDto>(contract);
    }
}
