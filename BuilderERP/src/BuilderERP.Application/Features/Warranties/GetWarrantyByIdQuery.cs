using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Warranties;

public record GetWarrantyByIdQuery(long Id) : IRequest<WarrantyDto?>;

public class GetWarrantyByIdQueryHandler : IRequestHandler<GetWarrantyByIdQuery, WarrantyDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWarrantyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<WarrantyDto?> Handle(GetWarrantyByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<Warranty>().Query()
            .Include(x => x.PropertyUnit)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<WarrantyDto>(item);
    }
}
