using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DefectRecords;

public record GetAllDefectRecordsQuery : IRequest<IReadOnlyList<DefectRecordDto>>;

public class GetAllDefectRecordsQueryHandler : IRequestHandler<GetAllDefectRecordsQuery, IReadOnlyList<DefectRecordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDefectRecordsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DefectRecordDto>> Handle(GetAllDefectRecordsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<DefectRecord>().Query()
            .Include(x => x.PropertyUnit)
            .OrderBy(x => x.DefectNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DefectRecordDto>>(items);
    }
}
