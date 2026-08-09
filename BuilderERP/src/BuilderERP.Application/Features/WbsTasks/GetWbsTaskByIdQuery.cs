using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.WbsTasks;

public record GetWbsTaskByIdQuery(Guid Id) : IRequest<WbsTaskDto?>;

public class GetWbsTaskByIdQueryHandler : IRequestHandler<GetWbsTaskByIdQuery, WbsTaskDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWbsTaskByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<WbsTaskDto?> Handle(GetWbsTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var wbsTask = await _unitOfWork.Repository<WbsTask>().Query()
            .Include(x => x.Project)
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return wbsTask is null ? null : _mapper.Map<WbsTaskDto>(wbsTask);
    }
}
