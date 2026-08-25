using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrderRequisitions;

public record GetAllEngineerWorkOrderRequisitionsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<EngineerWorkOrderRequisitionDto>>;

public class GetAllEngineerWorkOrderRequisitionsQueryHandler : IRequestHandler<GetAllEngineerWorkOrderRequisitionsQuery, PagedResult<EngineerWorkOrderRequisitionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEngineerWorkOrderRequisitionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<EngineerWorkOrderRequisitionDto>> Handle(GetAllEngineerWorkOrderRequisitionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<EngineerWorkOrderRequisition>().Query()
            .Include(r => r.Project)
            .Include(r => r.Details)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var requisitions = await query
            .OrderByDescending(r => r.RequestDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<EngineerWorkOrderRequisitionDto>>(requisitions);
        return new PagedResult<EngineerWorkOrderRequisitionDto>(items, totalCount, page, pageSize);
    }
}
