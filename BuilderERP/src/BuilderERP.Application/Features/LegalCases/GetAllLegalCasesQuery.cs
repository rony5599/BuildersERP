using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LegalCases;

public record GetAllLegalCasesQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<LegalCaseDto>>;

public class GetAllLegalCasesQueryHandler : IRequestHandler<GetAllLegalCasesQuery, PagedResult<LegalCaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLegalCasesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<LegalCaseDto>> Handle(GetAllLegalCasesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<LegalCase>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.CaseNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<LegalCaseDto>>(items);
        return new PagedResult<LegalCaseDto>(dtos, totalCount, page, pageSize);
    }
}
