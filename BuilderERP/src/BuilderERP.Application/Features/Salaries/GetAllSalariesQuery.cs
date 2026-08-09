using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Salaries;

public record GetAllSalariesQuery(Guid? WorkerId = null) : IRequest<IReadOnlyList<SalaryDto>>;

public class GetAllSalariesQueryHandler : IRequestHandler<GetAllSalariesQuery, IReadOnlyList<SalaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSalariesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SalaryDto>> Handle(GetAllSalariesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Salary>().Query()
            .Include(x => x.Worker)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var salaries = await query
            .OrderByDescending(x => x.PeriodStart)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SalaryDto>>(salaries);
    }
}
