using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Commissions;

public record GetCommissionByIdQuery(long Id) : IRequest<CommissionDto?>;

public class GetCommissionByIdQueryHandler : IRequestHandler<GetCommissionByIdQuery, CommissionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCommissionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CommissionDto?> Handle(GetCommissionByIdQuery request, CancellationToken cancellationToken)
    {
        var commission = await _unitOfWork.Repository<Commission>().GetByIdAsync(request.Id);
        return commission is null ? null : _mapper.Map<CommissionDto>(commission);
    }
}
