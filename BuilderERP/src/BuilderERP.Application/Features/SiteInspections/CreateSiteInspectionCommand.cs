using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteInspections;

public record CreateSiteInspectionCommand(CreateSiteInspectionDto Dto) : IRequest<Guid>;

public class CreateSiteInspectionCommandHandler : IRequestHandler<CreateSiteInspectionCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSiteInspectionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateSiteInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = _mapper.Map<SiteInspection>(request.Dto);
        await _unitOfWork.Repository<SiteInspection>().AddAsync(inspection);
        await _unitOfWork.SaveChangesAsync();
        return inspection.Id;
    }
}
