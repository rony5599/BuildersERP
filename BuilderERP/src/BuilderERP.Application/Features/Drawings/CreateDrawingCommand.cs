using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Drawings;

public record CreateDrawingCommand(CreateDrawingDto Dto) : IRequest<Guid>;

public class CreateDrawingCommandHandler : IRequestHandler<CreateDrawingCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDrawingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateDrawingCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<Drawing>(request.Dto);
        await _unitOfWork.Repository<Drawing>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
