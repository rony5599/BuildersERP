using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.FlatHandovers;

public record GetFlatHandoverByIdQuery(long Id) : IRequest<FlatHandoverDto?>;

public class GetFlatHandoverByIdQueryHandler : IRequestHandler<GetFlatHandoverByIdQuery, FlatHandoverDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFlatHandoverByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FlatHandoverDto?> Handle(GetFlatHandoverByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<FlatHandover>().Query()
            .Include(x => x.PropertyUnit)
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<FlatHandoverDto>(item);
    }
}
