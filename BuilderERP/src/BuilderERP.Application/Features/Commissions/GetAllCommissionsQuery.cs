using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Commissions;

public record GetAllCommissionsQuery : IRequest<IReadOnlyList<CommissionDto>>;

public class GetAllCommissionsQueryHandler : IRequestHandler<GetAllCommissionsQuery, IReadOnlyList<CommissionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCommissionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CommissionDto>> Handle(GetAllCommissionsQuery request, CancellationToken cancellationToken)
    {
        var commissions = await _unitOfWork.Repository<Commission>().Query()
            .Include(c => c.Broker)
            .Include(c => c.Booking)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CommissionDto>>(commissions);
    }
}
