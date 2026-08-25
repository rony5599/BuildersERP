using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Documents;

public record CreateDocumentCommand(CreateDocumentDto Dto) : IRequest<long>;

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDocumentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Document>(request.Dto);
        var repository = _unitOfWork.Repository<Document>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
