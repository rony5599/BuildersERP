using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PpeTrackings;

public record GetPpeTrackingByIdQuery(long Id) : IRequest<PpeTrackingDto?>;

public class GetPpeTrackingByIdQueryHandler : IRequestHandler<GetPpeTrackingByIdQuery, PpeTrackingDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPpeTrackingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PpeTrackingDto?> Handle(GetPpeTrackingByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<PpeTracking>().GetByIdAsync(request.Id);
        return item is null ? null : _mapper.Map<PpeTrackingDto>(item);
    }
}
