using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Warehouses;

public record CreateWarehouseCommand(CreateWarehouseDto Dto) : IRequest<Guid>;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWarehouseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = _mapper.Map<Warehouse>(request.Dto);
        await _unitOfWork.Repository<Warehouse>().AddAsync(warehouse);
        await _unitOfWork.SaveChangesAsync();
        return warehouse.Id;
    }
}
