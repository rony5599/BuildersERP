using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SecurityDeposits;

public record GetSecurityDepositByIdQuery(Guid Id) : IRequest<SecurityDepositDto?>;

public class GetSecurityDepositByIdQueryHandler : IRequestHandler<GetSecurityDepositByIdQuery, SecurityDepositDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSecurityDepositByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SecurityDepositDto?> Handle(GetSecurityDepositByIdQuery request, CancellationToken cancellationToken)
    {
        var deposit = await _unitOfWork.Repository<SecurityDeposit>().GetByIdAsync(request.Id);
        return deposit is null ? null : _mapper.Map<SecurityDepositDto>(deposit);
    }
}
