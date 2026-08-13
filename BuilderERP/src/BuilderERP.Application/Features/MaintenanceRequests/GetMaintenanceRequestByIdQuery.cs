using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.MaintenanceRequests;

public record GetMaintenanceRequestByIdQuery(Guid Id) : IRequest<MaintenanceRequestDto?>;

public class GetMaintenanceRequestByIdQueryHandler : IRequestHandler<GetMaintenanceRequestByIdQuery, MaintenanceRequestDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMaintenanceRequestByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MaintenanceRequestDto?> Handle(GetMaintenanceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<MaintenanceRequest>().Query()
            .Include(x => x.PropertyUnit)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<MaintenanceRequestDto>(item);
    }
}
