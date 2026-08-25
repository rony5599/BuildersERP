using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.VisitorLogs;

public record CreateVisitorLogCommand(CreateVisitorLogDto Dto) : IRequest<long>;

public class CreateVisitorLogCommandHandler : IRequestHandler<CreateVisitorLogCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateVisitorLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateVisitorLogCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<VisitorLog>(request.Dto);
        var repository = _unitOfWork.Repository<VisitorLog>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
