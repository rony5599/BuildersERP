using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Designations;

public record UpdateDesignationCommand(UpdateDesignationDto Dto) : IRequest<bool>;

public class UpdateDesignationCommandHandler : IRequestHandler<UpdateDesignationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDesignationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDesignationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Designation>();
        var designation = await repository.GetByIdAsync(request.Dto.Id);
        if (designation is null)
        {
            return false;
        }

        designation.Name = request.Dto.Name;
        designation.Code = request.Dto.Code;

        repository.Update(designation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
