using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PunchLists;

public record CreatePunchListCommand(CreatePunchListDto Dto) : IRequest<long>;

public class CreatePunchListCommandHandler : IRequestHandler<CreatePunchListCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePunchListCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreatePunchListCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<PunchList>(request.Dto);
        await _unitOfWork.Repository<PunchList>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
