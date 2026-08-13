using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandDocuments;

public record GetAllLandDocumentsQuery : IRequest<IReadOnlyList<LandDocumentDto>>;

public class GetAllLandDocumentsQueryHandler : IRequestHandler<GetAllLandDocumentsQuery, IReadOnlyList<LandDocumentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLandDocumentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LandDocumentDto>> Handle(GetAllLandDocumentsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<LandDocument>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.DocumentNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LandDocumentDto>>(items);
    }
}
