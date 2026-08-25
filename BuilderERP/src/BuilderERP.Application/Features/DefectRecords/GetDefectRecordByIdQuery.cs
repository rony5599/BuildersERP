using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DefectRecords;

public record GetDefectRecordByIdQuery(long Id) : IRequest<DefectRecordDto?>;

public class GetDefectRecordByIdQueryHandler : IRequestHandler<GetDefectRecordByIdQuery, DefectRecordDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDefectRecordByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DefectRecordDto?> Handle(GetDefectRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<DefectRecord>().Query()
            .Include(x => x.PropertyUnit)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<DefectRecordDto>(item);
    }
}
