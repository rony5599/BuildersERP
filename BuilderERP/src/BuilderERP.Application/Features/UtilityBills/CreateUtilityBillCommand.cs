using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.UtilityBills;

public record CreateUtilityBillCommand(CreateUtilityBillDto Dto) : IRequest<Guid>;

public class CreateUtilityBillCommandHandler : IRequestHandler<CreateUtilityBillCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUtilityBillCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateUtilityBillCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UtilityBill>(request.Dto);
        var repository = _unitOfWork.Repository<UtilityBill>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
