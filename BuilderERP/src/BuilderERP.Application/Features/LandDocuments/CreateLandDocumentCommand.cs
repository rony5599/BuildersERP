using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandDocuments;

public record CreateLandDocumentCommand(CreateLandDocumentDto Dto) : IRequest<Guid>;

public class CreateLandDocumentCommandHandler : IRequestHandler<CreateLandDocumentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLandDocumentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateLandDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<LandDocument>(request.Dto);
        var repository = _unitOfWork.Repository<LandDocument>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
