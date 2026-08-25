using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandRegistrations;

public record GetLandRegistrationByIdQuery(long Id) : IRequest<LandRegistrationDto?>;

public class GetLandRegistrationByIdQueryHandler : IRequestHandler<GetLandRegistrationByIdQuery, LandRegistrationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLandRegistrationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LandRegistrationDto?> Handle(GetLandRegistrationByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<LandRegistration>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<LandRegistrationDto>(item);
    }
}
