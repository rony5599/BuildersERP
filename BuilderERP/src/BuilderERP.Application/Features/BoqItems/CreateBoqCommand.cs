using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BoqItems;

public record CreateBoqCommand(CreateBoqDto Dto) : IRequest<long>;

public class CreateBoqCommandHandler : IRequestHandler<CreateBoqCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBoqCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<long> Handle(CreateBoqCommand request, CancellationToken cancellationToken)
    {
        var header = new BoqHeader
        {
            ProjectId = request.Dto.ProjectId,
            BoqName = request.Dto.BoqName.Trim(),
            VersionNumber = request.Dto.VersionNumber,
            ContingencyPercent = request.Dto.ContingencyPercent,
            IsActive = true,
            Items = request.Dto.Items.Select(x => new BoqItem
            {
                WorkGroupId = x.WorkGroupId,
                Description = x.Description.Trim(),
                UnitOfMeasure = x.UnitOfMeasure,
                Quantity = x.Quantity,
                Rate = x.Rate
            }).ToList()
        };

        await _unitOfWork.Repository<BoqHeader>().AddAsync(header);
        await _unitOfWork.SaveChangesAsync();
        return header.Id;
    }
}
