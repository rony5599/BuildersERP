using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandMutations;

public record GetAllLandMutationsQuery : IRequest<IReadOnlyList<LandMutationDto>>;

public class GetAllLandMutationsQueryHandler : IRequestHandler<GetAllLandMutationsQuery, IReadOnlyList<LandMutationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLandMutationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LandMutationDto>> Handle(GetAllLandMutationsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<LandMutation>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.MutationNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LandMutationDto>>(items);
    }
}
