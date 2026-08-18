using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteVisits;

public record CreateSiteVisitCommand(CreateSiteVisitDto Dto) : IRequest<Guid>;

public class CreateSiteVisitCommandHandler : IRequestHandler<CreateSiteVisitCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSiteVisitCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateSiteVisitCommand request, CancellationToken cancellationToken)
    {
        var visit = _mapper.Map<SiteVisit>(request.Dto);
        await _unitOfWork.Repository<SiteVisit>().AddAsync(visit);
        await _unitOfWork.SaveChangesAsync();

        return visit.Id;
    }
}
