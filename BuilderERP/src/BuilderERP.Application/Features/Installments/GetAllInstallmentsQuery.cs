using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Installments;

public record GetAllInstallmentsQuery : IRequest<IReadOnlyList<InstallmentDto>>;

public class GetAllInstallmentsQueryHandler : IRequestHandler<GetAllInstallmentsQuery, IReadOnlyList<InstallmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInstallmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InstallmentDto>> Handle(GetAllInstallmentsQuery request, CancellationToken cancellationToken)
    {
        var installments = await _unitOfWork.Repository<Installment>().GetAllAsync();
        return _mapper.Map<IReadOnlyList<InstallmentDto>>(installments);
    }
}
