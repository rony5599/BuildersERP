using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.WbsTasks;

public record CreateWbsTaskCommand(CreateWbsTaskDto Dto) : IRequest<Guid>;

public class CreateWbsTaskCommandHandler : IRequestHandler<CreateWbsTaskCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWbsTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateWbsTaskCommand request, CancellationToken cancellationToken)
    {
        var wbsTask = _mapper.Map<WbsTask>(request.Dto);
        await _unitOfWork.Repository<WbsTask>().AddAsync(wbsTask);
        await _unitOfWork.SaveChangesAsync();
        return wbsTask.Id;
    }
}
