using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DrawingRevisions;

public record CreateDrawingRevisionCommand(CreateDrawingRevisionDto Dto) : IRequest<long>;

public class CreateDrawingRevisionCommandHandler : IRequestHandler<CreateDrawingRevisionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDrawingRevisionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateDrawingRevisionCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<DrawingRevision>(request.Dto);
        var repository = _unitOfWork.Repository<DrawingRevision>();

        if (entity.IsCurrent)
        {
            var otherRevisions = await repository.Query()
                .Where(r => r.DrawingId == entity.DrawingId)
                .ToListAsync(cancellationToken);

            foreach (var revision in otherRevisions)
            {
                revision.IsCurrent = false;
                repository.Update(revision);
            }
        }

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
