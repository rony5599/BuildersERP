using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Leads;

public record UpdateLeadCommand(UpdateLeadDto Dto) : IRequest<bool>;

public class UpdateLeadCommandHandler : IRequestHandler<UpdateLeadCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLeadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLeadCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Lead>();
        var lead = await repository.GetByIdAsync(request.Dto.Id);
        if (lead is null)
        {
            return false;
        }

        lead.Name = request.Dto.Name;
        lead.Phone = request.Dto.Phone;
        lead.Email = request.Dto.Email;
        lead.Source = request.Dto.Source;
        lead.Status = request.Dto.Status;
        lead.Notes = request.Dto.Notes;
        lead.AssignedToUserId = request.Dto.AssignedToUserId;

        repository.Update(lead);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
