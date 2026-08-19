using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Contractors;

public record GetAllContractorsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<ContractorDto>>;

public class GetAllContractorsQueryHandler : IRequestHandler<GetAllContractorsQuery, PagedResult<ContractorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllContractorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ContractorDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Contractor>().Query().AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var contractors = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<ContractorDto>>(contractors);
        return new PagedResult<ContractorDto>(items, totalCount, page, pageSize);
    }
}
