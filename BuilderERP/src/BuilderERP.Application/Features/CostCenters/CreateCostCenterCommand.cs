using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CostCenters;

public record CreateCostCenterCommand(CreateCostCenterDto Dto) : IRequest<long>;

public class CreateCostCenterCommandHandler : IRequestHandler<CreateCostCenterCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCostCenterCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateCostCenterCommand request, CancellationToken cancellationToken)
    {
        var costCenter = _mapper.Map<CostCenter>(request.Dto);
        await _unitOfWork.Repository<CostCenter>().AddAsync(costCenter);
        await _unitOfWork.SaveChangesAsync();

        return costCenter.Id;
    }
}
