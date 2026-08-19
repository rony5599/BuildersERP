using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LandRegistrations;

public record GetAllLandRegistrationsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<LandRegistrationDto>>;

public class GetAllLandRegistrationsQueryHandler : IRequestHandler<GetAllLandRegistrationsQuery, PagedResult<LandRegistrationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLandRegistrationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<LandRegistrationDto>> Handle(GetAllLandRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<LandRegistration>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.RegistrationNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<LandRegistrationDto>>(items);
        return new PagedResult<LandRegistrationDto>(dtos, totalCount, page, pageSize);
    }
}
