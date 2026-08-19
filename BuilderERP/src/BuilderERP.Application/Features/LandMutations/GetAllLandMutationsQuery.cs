using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandMutations;

public record GetAllLandMutationsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<LandMutationDto>>;

public class GetAllLandMutationsQueryHandler : IRequestHandler<GetAllLandMutationsQuery, PagedResult<LandMutationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLandMutationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<LandMutationDto>> Handle(GetAllLandMutationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<LandMutation>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.MutationNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<LandMutationDto>>(items);
        return new PagedResult<LandMutationDto>(dtos, totalCount, page, pageSize);
    }
}
