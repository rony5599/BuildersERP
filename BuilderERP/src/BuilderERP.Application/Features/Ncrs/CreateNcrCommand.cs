using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Ncrs;

public record CreateNcrCommand(CreateNcrDto Dto) : IRequest<long>;

public class CreateNcrCommandHandler : IRequestHandler<CreateNcrCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateNcrCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateNcrCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<Ncr>(request.Dto);
        await _unitOfWork.Repository<Ncr>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
