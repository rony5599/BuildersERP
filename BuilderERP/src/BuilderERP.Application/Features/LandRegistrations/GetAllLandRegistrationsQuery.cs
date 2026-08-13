using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandRegistrations;

public record GetAllLandRegistrationsQuery : IRequest<IReadOnlyList<LandRegistrationDto>>;

public class GetAllLandRegistrationsQueryHandler : IRequestHandler<GetAllLandRegistrationsQuery, IReadOnlyList<LandRegistrationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLandRegistrationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LandRegistrationDto>> Handle(GetAllLandRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<LandRegistration>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.RegistrationNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LandRegistrationDto>>(items);
    }
}
