using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Workers;

public record GetAllWorkersQuery(Guid? ContractorId = null) : IRequest<IReadOnlyList<WorkerDto>>;

public class GetAllWorkersQueryHandler : IRequestHandler<GetAllWorkersQuery, IReadOnlyList<WorkerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWorkersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WorkerDto>> Handle(GetAllWorkersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Worker>().Query()
            .Include(x => x.Contractor)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var workers = await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<WorkerDto>>(workers);
    }
}
