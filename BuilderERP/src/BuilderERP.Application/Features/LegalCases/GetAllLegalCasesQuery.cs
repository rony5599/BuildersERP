using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LegalCases;

public record GetAllLegalCasesQuery : IRequest<IReadOnlyList<LegalCaseDto>>;

public class GetAllLegalCasesQueryHandler : IRequestHandler<GetAllLegalCasesQuery, IReadOnlyList<LegalCaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLegalCasesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LegalCaseDto>> Handle(GetAllLegalCasesQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<LegalCase>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.CaseNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LegalCaseDto>>(items);
    }
}
