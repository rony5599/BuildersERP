using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VisitorLogs;

public record GetVisitorLogByIdQuery(Guid Id) : IRequest<VisitorLogDto?>;

public class GetVisitorLogByIdQueryHandler : IRequestHandler<GetVisitorLogByIdQuery, VisitorLogDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVisitorLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VisitorLogDto?> Handle(GetVisitorLogByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<VisitorLog>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<VisitorLogDto>(item);
    }
}
