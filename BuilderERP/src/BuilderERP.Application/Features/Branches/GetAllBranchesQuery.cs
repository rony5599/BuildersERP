using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Branches;

public record GetAllBranchesQuery : IRequest<IReadOnlyList<BranchDto>>;

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, IReadOnlyList<BranchDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBranchesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await _unitOfWork.Repository<Branch>().Query().Include(b => b.Company).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BranchDto>>(branches);
    }
}
