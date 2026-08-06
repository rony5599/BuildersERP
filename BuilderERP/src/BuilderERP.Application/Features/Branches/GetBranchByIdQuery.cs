using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Branches;

public record GetBranchByIdQuery(Guid Id) : IRequest<BranchDto?>;

public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBranchByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BranchDto?> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(request.Id);
        return branch is null ? null : _mapper.Map<BranchDto>(branch);
    }
}
