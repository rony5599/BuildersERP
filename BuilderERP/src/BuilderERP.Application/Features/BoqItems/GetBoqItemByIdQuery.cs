using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BoqItems;

public record GetBoqItemByIdQuery(long Id) : IRequest<BoqItemDto?>;

public class GetBoqItemByIdQueryHandler : IRequestHandler<GetBoqItemByIdQuery, BoqItemDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBoqItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BoqItemDto?> Handle(GetBoqItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<BoqItem>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<BoqItemDto>(item);
    }
}
