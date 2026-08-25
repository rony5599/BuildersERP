using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteInspections;

public record CreateSiteInspectionCommand(CreateSiteInspectionDto Dto) : IRequest<long>;

public class CreateSiteInspectionCommandHandler : IRequestHandler<CreateSiteInspectionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSiteInspectionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateSiteInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = _mapper.Map<SiteInspection>(request.Dto);
        await _unitOfWork.Repository<SiteInspection>().AddAsync(inspection);
        await _unitOfWork.SaveChangesAsync();
        return inspection.Id;
    }
}
