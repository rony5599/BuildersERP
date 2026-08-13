using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.FlatHandovers;

public record GetAllFlatHandoversQuery : IRequest<IReadOnlyList<FlatHandoverDto>>;

public class GetAllFlatHandoversQueryHandler : IRequestHandler<GetAllFlatHandoversQuery, IReadOnlyList<FlatHandoverDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllFlatHandoversQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FlatHandoverDto>> Handle(GetAllFlatHandoversQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<FlatHandover>().Query()
            .Include(x => x.PropertyUnit)
            .Include(x => x.Customer)
            .OrderBy(x => x.HandoverNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FlatHandoverDto>>(items);
    }
}
