using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SnagItems;

public record GetAllSnagItemsQuery : IRequest<IReadOnlyList<SnagItemDto>>;

public class GetAllSnagItemsQueryHandler : IRequestHandler<GetAllSnagItemsQuery, IReadOnlyList<SnagItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSnagItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SnagItemDto>> Handle(GetAllSnagItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<SnagItem>().Query()
            .Include(x => x.PropertyUnit)
            .OrderBy(x => x.SnagNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SnagItemDto>>(items);
    }
}
