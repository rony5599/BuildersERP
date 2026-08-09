using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DailyProgresses;

public record CreateDailyProgressCommand(CreateDailyProgressDto Dto) : IRequest<Guid>;

public class CreateDailyProgressCommandHandler : IRequestHandler<CreateDailyProgressCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDailyProgressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<DailyProgress>(request.Dto);
        await _unitOfWork.Repository<DailyProgress>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
