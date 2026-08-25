using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Leads;

public record CreateLeadCommand(CreateLeadDto Dto) : IRequest<long>;

public class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLeadCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = _mapper.Map<Lead>(request.Dto);
        await _unitOfWork.Repository<Lead>().AddAsync(lead);
        await _unitOfWork.SaveChangesAsync();

        return lead.Id;
    }
}
