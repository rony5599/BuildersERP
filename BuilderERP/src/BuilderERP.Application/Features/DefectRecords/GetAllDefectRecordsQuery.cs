using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DefectRecords;

public record GetAllDefectRecordsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<DefectRecordDto>>;

public class GetAllDefectRecordsQueryHandler : IRequestHandler<GetAllDefectRecordsQuery, PagedResult<DefectRecordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDefectRecordsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DefectRecordDto>> Handle(GetAllDefectRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DefectRecord>().Query()
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.DefectNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<DefectRecordDto>>(results);
        return new PagedResult<DefectRecordDto>(items, totalCount, page, pageSize);
    }
}
