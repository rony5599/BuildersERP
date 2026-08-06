using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CostCenters;

public record GetAllCostCentersQuery : IRequest<IReadOnlyList<CostCenterDto>>;

public class GetAllCostCentersQueryHandler : IRequestHandler<GetAllCostCentersQuery, IReadOnlyList<CostCenterDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCostCentersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CostCenterDto>> Handle(GetAllCostCentersQuery request, CancellationToken cancellationToken)
    {
        var costCenters = await _unitOfWork.Repository<CostCenter>().Query().Include(c => c.Project).ToListAsync();
        return _mapper.Map<IReadOnlyList<CostCenterDto>>(costCenters);
    }
}
