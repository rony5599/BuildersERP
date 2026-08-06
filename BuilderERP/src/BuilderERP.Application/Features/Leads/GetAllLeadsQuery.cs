using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Leads;

public record GetAllLeadsQuery : IRequest<IReadOnlyList<LeadDto>>;

public class GetAllLeadsQueryHandler : IRequestHandler<GetAllLeadsQuery, IReadOnlyList<LeadDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLeadsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LeadDto>> Handle(GetAllLeadsQuery request, CancellationToken cancellationToken)
    {
        var leads = await _unitOfWork.Repository<Lead>().Query().Include(l => l.AssignedToUser).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LeadDto>>(leads);
    }
}
