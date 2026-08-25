using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Suppliers;

public record CreateSupplierCommand(CreateSupplierDto Dto) : IRequest<long>;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSupplierCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = _mapper.Map<Supplier>(request.Dto);
        await _unitOfWork.Repository<Supplier>().AddAsync(supplier);
        await _unitOfWork.SaveChangesAsync();

        return supplier.Id;
    }
}
