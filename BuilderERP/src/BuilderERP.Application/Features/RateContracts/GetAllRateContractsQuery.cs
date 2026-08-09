using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.RateContracts;

public record GetAllRateContractsQuery(Guid? ContractorId = null) : IRequest<IReadOnlyList<RateContractDto>>;

public class GetAllRateContractsQueryHandler : IRequestHandler<GetAllRateContractsQuery, IReadOnlyList<RateContractDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRateContractsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RateContractDto>> Handle(GetAllRateContractsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<RateContract>().Query()
            .Include(x => x.Contractor)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var contracts = await query
            .OrderBy(x => x.ContractNumber)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<RateContractDto>>(contracts);
    }
}
