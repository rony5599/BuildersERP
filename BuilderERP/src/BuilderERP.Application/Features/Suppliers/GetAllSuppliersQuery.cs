using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Suppliers;

public record GetAllSuppliersQuery : IRequest<IReadOnlyList<SupplierDto>>;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, IReadOnlyList<SupplierDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSuppliersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _unitOfWork.Repository<Supplier>().Query().ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SupplierDto>>(suppliers);
    }
}
