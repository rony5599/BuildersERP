using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DocumentVersions;

public record CreateDocumentVersionCommand(CreateDocumentVersionDto Dto) : IRequest<long>;

public class CreateDocumentVersionCommandHandler : IRequestHandler<CreateDocumentVersionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDocumentVersionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateDocumentVersionCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<DocumentVersion>(request.Dto);
        var repository = _unitOfWork.Repository<DocumentVersion>();

        if (entity.IsCurrent)
        {
            var otherVersions = await repository.Query()
                .Where(v => v.DocumentId == entity.DocumentId)
                .ToListAsync(cancellationToken);

            foreach (var version in otherVersions)
            {
                version.IsCurrent = false;
                repository.Update(version);
            }
        }

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
