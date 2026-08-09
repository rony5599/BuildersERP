using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Contractors;

public record CreateContractorCommand(CreateContractorDto Dto) : IRequest<Guid>;

public class CreateContractorCommandHandler : IRequestHandler<CreateContractorCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateContractorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = _mapper.Map<Contractor>(request.Dto);
        await _unitOfWork.Repository<Contractor>().AddAsync(contractor);
        await _unitOfWork.SaveChangesAsync();

        return contractor.Id;
    }
}
