using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VisitorLogs;

public record GetAllVisitorLogsQuery : IRequest<IReadOnlyList<VisitorLogDto>>;

public class GetAllVisitorLogsQueryHandler : IRequestHandler<GetAllVisitorLogsQuery, IReadOnlyList<VisitorLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllVisitorLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<VisitorLogDto>> Handle(GetAllVisitorLogsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<VisitorLog>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.VisitorName)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<VisitorLogDto>>(items);
    }
}
