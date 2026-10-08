using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BoqItems;

public record GetBoqForEditQuery(long Id) : IRequest<UpdateBoqDto?>;

public class GetBoqForEditQueryHandler : IRequestHandler<GetBoqForEditQuery, UpdateBoqDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetBoqForEditQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<UpdateBoqDto?> Handle(GetBoqForEditQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<BoqHeader>().Query().AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new UpdateBoqDto
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                BoqName = x.BoqName,
                VersionNumber = x.VersionNumber,
                ContingencyPercent = x.ContingencyPercent,
                IsActive = x.IsActive,
                Status = x.Status,
                RejectionReason = x.RejectionReason,
                RejectedBy = x.RejectedBy,
                RejectedAt = x.RejectedAt,
                Items = x.Items.OrderBy(i => i.Id).Select(i => new CreateBoqItemDto
                {
                    WorkGroupId = i.WorkGroupId,
                    Description = i.Description,
                    UnitOfMeasure = i.UnitOfMeasure,
                    Quantity = i.Quantity,
                    Rate = i.Rate
                }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);
    }
}

public record UpdateBoqCommand(UpdateBoqDto Dto) : IRequest<bool>;

public class UpdateBoqCommandHandler : IRequestHandler<UpdateBoqCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateBoqCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateBoqCommand request, CancellationToken cancellationToken)
    {
        var headerRepository = _unitOfWork.Repository<BoqHeader>();
        var itemRepository = _unitOfWork.Repository<BoqItem>();
        var header = await headerRepository.Query().Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Dto.Id, cancellationToken);
        if (header is null) return false;
        if (header.Status != Domain.Enums.BoqStatus.Draft) return false;

        header.ProjectId = request.Dto.ProjectId;
        header.BoqName = request.Dto.BoqName.Trim();
        header.VersionNumber = request.Dto.VersionNumber;
        header.ContingencyPercent = request.Dto.ContingencyPercent;

        foreach (var oldItem in header.Items.ToList()) itemRepository.Remove(oldItem);
        header.Items.Clear();
        foreach (var item in request.Dto.Items)
        {
            header.Items.Add(new BoqItem
            {
                WorkGroupId = item.WorkGroupId,
                Description = item.Description.Trim(),
                UnitOfMeasure = item.UnitOfMeasure,
                Quantity = item.Quantity,
                Rate = item.Rate
            });
        }

        headerRepository.Update(header);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
