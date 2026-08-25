using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Ncrs;

public record GetNcrByIdQuery(long Id) : IRequest<NcrDto?>;

public class GetNcrByIdQueryHandler : IRequestHandler<GetNcrByIdQuery, NcrDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetNcrByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NcrDto?> Handle(GetNcrByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<Ncr>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<NcrDto>(item);
    }
}
