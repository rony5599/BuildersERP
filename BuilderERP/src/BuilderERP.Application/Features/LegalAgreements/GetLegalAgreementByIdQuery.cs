using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LegalAgreements;

public record GetLegalAgreementByIdQuery(Guid Id) : IRequest<LegalAgreementDto?>;

public class GetLegalAgreementByIdQueryHandler : IRequestHandler<GetLegalAgreementByIdQuery, LegalAgreementDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLegalAgreementByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LegalAgreementDto?> Handle(GetLegalAgreementByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<LegalAgreement>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<LegalAgreementDto>(item);
    }
}
