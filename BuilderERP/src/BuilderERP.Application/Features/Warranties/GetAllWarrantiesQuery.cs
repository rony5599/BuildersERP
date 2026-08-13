using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Warranties;

public record GetAllWarrantiesQuery : IRequest<IReadOnlyList<WarrantyDto>>;

public class GetAllWarrantiesQueryHandler : IRequestHandler<GetAllWarrantiesQuery, IReadOnlyList<WarrantyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWarrantiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WarrantyDto>> Handle(GetAllWarrantiesQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<Warranty>().Query()
            .Include(x => x.PropertyUnit)
            .OrderBy(x => x.WarrantyNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<WarrantyDto>>(items);
    }
}
