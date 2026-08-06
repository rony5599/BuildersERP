using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Application.DTOs;
using MediatR;

namespace BuilderERP.Application.Features.Projects;

public record UpdateProjectCommand(UpdateProjectDto Dto) : IRequest<bool>;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Project>();
        var project = await repository.GetByIdAsync(request.Dto.Id);
        if (project is null)
        {
            return false;
        }

        project.Name = request.Dto.Name;
        project.Code = request.Dto.Code;
        project.Location = request.Dto.Location;
        project.StartDate = request.Dto.StartDate;
        project.EndDate = request.Dto.EndDate;
        project.BranchId = request.Dto.BranchId;

        repository.Update(project);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
