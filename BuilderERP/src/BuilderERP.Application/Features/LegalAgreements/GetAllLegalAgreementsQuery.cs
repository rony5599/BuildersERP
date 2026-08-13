using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LegalAgreements;

public record GetAllLegalAgreementsQuery : IRequest<IReadOnlyList<LegalAgreementDto>>;

public class GetAllLegalAgreementsQueryHandler : IRequestHandler<GetAllLegalAgreementsQuery, IReadOnlyList<LegalAgreementDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLegalAgreementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LegalAgreementDto>> Handle(GetAllLegalAgreementsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<LegalAgreement>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.AgreementNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LegalAgreementDto>>(items);
    }
}
