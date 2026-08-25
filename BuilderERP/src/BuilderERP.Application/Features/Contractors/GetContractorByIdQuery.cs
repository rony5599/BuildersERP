using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Contractors;

public record GetContractorByIdQuery(long Id) : IRequest<ContractorDto?>;

public class GetContractorByIdQueryHandler : IRequestHandler<GetContractorByIdQuery, ContractorDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContractorByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContractorDto?> Handle(GetContractorByIdQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _unitOfWork.Repository<Contractor>().GetByIdAsync(request.Id);
        return contractor is null ? null : _mapper.Map<ContractorDto>(contractor);
    }
}
