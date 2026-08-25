using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DocumentVersions;

public record GetDocumentVersionByIdQuery(long Id) : IRequest<DocumentVersionDto?>;

public class GetDocumentVersionByIdQueryHandler : IRequestHandler<GetDocumentVersionByIdQuery, DocumentVersionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDocumentVersionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DocumentVersionDto?> Handle(GetDocumentVersionByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<DocumentVersion>().Query()
            .Include(x => x.Document)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<DocumentVersionDto>(item);
    }
}
