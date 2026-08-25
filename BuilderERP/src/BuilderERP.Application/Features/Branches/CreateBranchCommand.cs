using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Branches;

public record CreateBranchCommand(CreateBranchDto Dto) : IRequest<long>;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBranchCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = _mapper.Map<Branch>(request.Dto);
        await _unitOfWork.Repository<Branch>().AddAsync(branch);
        await _unitOfWork.SaveChangesAsync();

        return branch.Id;
    }
}
