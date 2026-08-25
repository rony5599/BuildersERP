using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BoqItems;

public record CreateBoqItemCommand(CreateBoqItemDto Dto) : IRequest<long>;

public class CreateBoqItemCommandHandler : IRequestHandler<CreateBoqItemCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBoqItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateBoqItemCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<BoqItem>(request.Dto);
        item.Amount = item.Quantity * item.Rate;
        await _unitOfWork.Repository<BoqItem>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
