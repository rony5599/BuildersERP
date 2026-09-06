using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Designations;

public record GetAllDesignationsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<DesignationDto>>;

public class GetAllDesignationsQueryHandler : IRequestHandler<GetAllDesignationsQuery, PagedResult<DesignationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDesignationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DesignationDto>> Handle(GetAllDesignationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Designation>().Query();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var designations = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<DesignationDto>>(designations);
        return new PagedResult<DesignationDto>(items, totalCount, page, pageSize);
    }
}
