using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Contractors;

public record GetAllContractorsQuery : IRequest<IReadOnlyList<ContractorDto>>;

public class GetAllContractorsQueryHandler : IRequestHandler<GetAllContractorsQuery, IReadOnlyList<ContractorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllContractorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ContractorDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        var contractors = await _unitOfWork.Repository<Contractor>().Query()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ContractorDto>>(contractors);
    }
}
