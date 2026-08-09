using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Workers;

public record CreateWorkerCommand(CreateWorkerDto Dto) : IRequest<Guid>;

public class CreateWorkerCommandHandler : IRequestHandler<CreateWorkerCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWorkerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateWorkerCommand request, CancellationToken cancellationToken)
    {
        var worker = _mapper.Map<Worker>(request.Dto);
        await _unitOfWork.Repository<Worker>().AddAsync(worker);
        await _unitOfWork.SaveChangesAsync();

        return worker.Id;
    }
}
