using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Projects;

public record SetProjectActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetProjectActiveCommandHandler : IRequestHandler<SetProjectActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetProjectActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetProjectActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Project>();
        var project = await repository.GetByIdAsync(request.Id);
        if (project is null)
        {
            return false;
        }

        project.IsActive = request.IsActive;
        repository.Update(project);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
