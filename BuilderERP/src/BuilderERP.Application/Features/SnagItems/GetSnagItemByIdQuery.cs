using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SnagItems;

public record GetSnagItemByIdQuery(Guid Id) : IRequest<SnagItemDto?>;

public class GetSnagItemByIdQueryHandler : IRequestHandler<GetSnagItemByIdQuery, SnagItemDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSnagItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SnagItemDto?> Handle(GetSnagItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<SnagItem>().Query()
            .Include(x => x.PropertyUnit)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<SnagItemDto>(item);
    }
}
