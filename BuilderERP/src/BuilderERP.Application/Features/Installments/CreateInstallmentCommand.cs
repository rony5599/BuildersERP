using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Installments;

public record CreateInstallmentCommand(CreateInstallmentDto Dto) : IRequest<Guid>;

public class CreateInstallmentCommandHandler : IRequestHandler<CreateInstallmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInstallmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateInstallmentCommand request, CancellationToken cancellationToken)
    {
        var installment = _mapper.Map<Installment>(request.Dto);
        await _unitOfWork.Repository<Installment>().AddAsync(installment);
        await _unitOfWork.SaveChangesAsync();

        return installment.Id;
    }
}
