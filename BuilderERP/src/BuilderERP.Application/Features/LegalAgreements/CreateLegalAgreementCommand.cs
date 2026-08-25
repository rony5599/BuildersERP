using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalAgreements;

public record CreateLegalAgreementCommand(CreateLegalAgreementDto Dto) : IRequest<long>;

public class CreateLegalAgreementCommandHandler : IRequestHandler<CreateLegalAgreementCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLegalAgreementCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateLegalAgreementCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<LegalAgreement>(request.Dto);
        var repository = _unitOfWork.Repository<LegalAgreement>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
