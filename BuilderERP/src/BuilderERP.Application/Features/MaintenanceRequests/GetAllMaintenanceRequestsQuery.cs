using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.MaintenanceRequests;

public record GetAllMaintenanceRequestsQuery : IRequest<IReadOnlyList<MaintenanceRequestDto>>;

public class GetAllMaintenanceRequestsQueryHandler : IRequestHandler<GetAllMaintenanceRequestsQuery, IReadOnlyList<MaintenanceRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaintenanceRequestsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MaintenanceRequestDto>> Handle(GetAllMaintenanceRequestsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<MaintenanceRequest>().Query()
            .Include(x => x.PropertyUnit)
            .OrderBy(x => x.RequestNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<MaintenanceRequestDto>>(items);
    }
}
