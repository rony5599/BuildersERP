using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LegalCases;

public record GetLegalCaseByIdQuery(long Id) : IRequest<LegalCaseDto?>;

public class GetLegalCaseByIdQueryHandler : IRequestHandler<GetLegalCaseByIdQuery, LegalCaseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLegalCaseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LegalCaseDto?> Handle(GetLegalCaseByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<LegalCase>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<LegalCaseDto>(item);
    }
}
