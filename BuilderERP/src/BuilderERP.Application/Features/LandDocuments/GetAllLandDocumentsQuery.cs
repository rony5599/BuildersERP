using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandDocuments;

public record GetAllLandDocumentsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<LandDocumentDto>>;

public class GetAllLandDocumentsQueryHandler : IRequestHandler<GetAllLandDocumentsQuery, PagedResult<LandDocumentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLandDocumentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<LandDocumentDto>> Handle(GetAllLandDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<LandDocument>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DocumentNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<LandDocumentDto>>(items);
        return new PagedResult<LandDocumentDto>(dtos, totalCount, page, pageSize);
    }
}
