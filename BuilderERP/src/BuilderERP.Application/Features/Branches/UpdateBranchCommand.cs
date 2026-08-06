using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Application.DTOs;
using MediatR;

namespace BuilderERP.Application.Features.Branches;

public record UpdateBranchCommand(UpdateBranchDto Dto) : IRequest<bool>;

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBranchCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Branch>();
        var branch = await repository.GetByIdAsync(request.Dto.Id);
        if (branch is null)
        {
            return false;
        }

        branch.Name = request.Dto.Name;
        branch.Code = request.Dto.Code;
        branch.Address = request.Dto.Address;
        branch.CompanyId = request.Dto.CompanyId;

        repository.Update(branch);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
