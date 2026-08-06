using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Companies;

public record SetCompanyActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetCompanyActiveCommandHandler : IRequestHandler<SetCompanyActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCompanyActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCompanyActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Company>();
        var company = await repository.GetByIdAsync(request.Id);
        if (company is null)
        {
            return false;
        }

        company.IsActive = request.IsActive;
        repository.Update(company);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
