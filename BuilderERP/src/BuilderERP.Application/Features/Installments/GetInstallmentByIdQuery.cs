using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Installments;

public record GetInstallmentByIdQuery(Guid Id) : IRequest<InstallmentDto?>;

public class GetInstallmentByIdQueryHandler : IRequestHandler<GetInstallmentByIdQuery, InstallmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetInstallmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InstallmentDto?> Handle(GetInstallmentByIdQuery request, CancellationToken cancellationToken)
    {
        var installment = await _unitOfWork.Repository<Installment>().GetByIdAsync(request.Id);
        return installment is null ? null : _mapper.Map<InstallmentDto>(installment);
    }
}
