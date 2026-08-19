using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Documents;

public record GetAllDocumentsQuery(Guid? CustomerId = null, Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<DocumentDto>>;

public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, PagedResult<DocumentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDocumentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Document>().Query()
            .Include(x => x.Customer)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == request.CustomerId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.DocumentNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<DocumentDto>>(results);
        return new PagedResult<DocumentDto>(items, totalCount, page, pageSize);
    }
}
