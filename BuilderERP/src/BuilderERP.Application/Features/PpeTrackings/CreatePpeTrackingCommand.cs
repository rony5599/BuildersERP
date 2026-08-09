using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PpeTrackings;

public record CreatePpeTrackingCommand(CreatePpeTrackingDto Dto) : IRequest<Guid>;

public class CreatePpeTrackingCommandHandler : IRequestHandler<CreatePpeTrackingCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePpeTrackingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreatePpeTrackingCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<PpeTracking>(request.Dto);
        await _unitOfWork.Repository<PpeTracking>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
