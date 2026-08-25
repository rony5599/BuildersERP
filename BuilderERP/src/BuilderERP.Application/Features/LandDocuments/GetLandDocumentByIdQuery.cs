using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandDocuments;

public record GetLandDocumentByIdQuery(long Id) : IRequest<LandDocumentDto?>;

public class GetLandDocumentByIdQueryHandler : IRequestHandler<GetLandDocumentByIdQuery, LandDocumentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLandDocumentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LandDocumentDto?> Handle(GetLandDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<LandDocument>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<LandDocumentDto>(item);
    }
}
