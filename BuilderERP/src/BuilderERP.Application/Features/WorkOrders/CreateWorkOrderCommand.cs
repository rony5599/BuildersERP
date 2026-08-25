using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.WorkOrders;

public record CreateWorkOrderCommand(CreateWorkOrderDto Dto) : IRequest<long>;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWorkOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = _mapper.Map<WorkOrder>(request.Dto);
        await _unitOfWork.Repository<WorkOrder>().AddAsync(workOrder);
        await _unitOfWork.SaveChangesAsync();
        return workOrder.Id;
    }
}
