using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DocumentVersions;

public record GetAllDocumentVersionsQuery(Guid? DocumentId = null) : IRequest<IReadOnlyList<DocumentVersionDto>>;

public class GetAllDocumentVersionsQueryHandler : IRequestHandler<GetAllDocumentVersionsQuery, IReadOnlyList<DocumentVersionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDocumentVersionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DocumentVersionDto>> Handle(GetAllDocumentVersionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DocumentVersion>().Query()
            .Include(x => x.Document)
            .AsQueryable();

        if (request.DocumentId.HasValue)
        {
            query = query.Where(x => x.DocumentId == request.DocumentId.Value);
        }

        var items = await query.OrderByDescending(x => x.UploadedDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DocumentVersionDto>>(items);
    }
}
