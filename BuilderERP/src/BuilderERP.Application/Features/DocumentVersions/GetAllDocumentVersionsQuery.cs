using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DocumentVersions;

public record GetAllDocumentVersionsQuery(Guid? DocumentId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<DocumentVersionDto>>;

public class GetAllDocumentVersionsQueryHandler : IRequestHandler<GetAllDocumentVersionsQuery, PagedResult<DocumentVersionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDocumentVersionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DocumentVersionDto>> Handle(GetAllDocumentVersionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DocumentVersion>().Query()
            .Include(x => x.Document)
            .AsQueryable();

        if (request.DocumentId.HasValue)
        {
            query = query.Where(x => x.DocumentId == request.DocumentId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderByDescending(x => x.UploadedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<DocumentVersionDto>>(results);
        return new PagedResult<DocumentVersionDto>(items, totalCount, page, pageSize);
    }
}
