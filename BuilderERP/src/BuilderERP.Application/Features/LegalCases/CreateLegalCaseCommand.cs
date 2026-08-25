using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalCases;

public record CreateLegalCaseCommand(CreateLegalCaseDto Dto) : IRequest<long>;

public class CreateLegalCaseCommandHandler : IRequestHandler<CreateLegalCaseCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLegalCaseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateLegalCaseCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<LegalCase>(request.Dto);
        var repository = _unitOfWork.Repository<LegalCase>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
