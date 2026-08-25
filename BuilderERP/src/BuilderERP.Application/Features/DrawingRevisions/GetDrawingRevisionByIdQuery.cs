using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DrawingRevisions;

public record GetDrawingRevisionByIdQuery(long Id) : IRequest<DrawingRevisionDto?>;

public class GetDrawingRevisionByIdQueryHandler : IRequestHandler<GetDrawingRevisionByIdQuery, DrawingRevisionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDrawingRevisionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DrawingRevisionDto?> Handle(GetDrawingRevisionByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<DrawingRevision>().Query()
            .Include(x => x.Drawing)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<DrawingRevisionDto>(item);
    }
}
