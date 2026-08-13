using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DefectRecords;

public record CreateDefectRecordCommand(CreateDefectRecordDto Dto) : IRequest<Guid>;

public class CreateDefectRecordCommandHandler : IRequestHandler<CreateDefectRecordCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDefectRecordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateDefectRecordCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<DefectRecord>(request.Dto);
        var repository = _unitOfWork.Repository<DefectRecord>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
