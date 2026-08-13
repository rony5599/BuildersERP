using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandMutations;

public record GetLandMutationByIdQuery(Guid Id) : IRequest<LandMutationDto?>;

public class GetLandMutationByIdQueryHandler : IRequestHandler<GetLandMutationByIdQuery, LandMutationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLandMutationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LandMutationDto?> Handle(GetLandMutationByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<LandMutation>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<LandMutationDto>(item);
    }
}
