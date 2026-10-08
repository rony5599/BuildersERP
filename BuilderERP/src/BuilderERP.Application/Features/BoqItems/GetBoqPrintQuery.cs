using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BoqItems;

public record GetBoqPrintQuery(long Id) : IRequest<BoqPrintDto?>;

public class GetBoqPrintQueryHandler : IRequestHandler<GetBoqPrintQuery, BoqPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetBoqPrintQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BoqPrintDto?> Handle(GetBoqPrintQuery request, CancellationToken cancellationToken)
    {
        var model = await _unitOfWork.Repository<BoqHeader>().Query().AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new BoqPrintDto
            {
                Id = x.Id,
                ProjectName = x.Project.Name,
                BoqName = x.BoqName,
                VersionNumber = x.VersionNumber,
                CreatedAt = x.CreatedAt,
                Status = x.Status,
                ContingencyPercent = x.ContingencyPercent,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt,
                RejectionReason = x.RejectionReason,
                RejectedBy = x.RejectedBy,
                RejectedAt = x.RejectedAt,
                Lines = x.Items.OrderBy(i => i.WorkGroup.GroupCode).ThenBy(i => i.Id)
                    .Select(i => new BoqPrintLineDto
                    {
                        WorkGroupCode = i.WorkGroup.GroupCode,
                        WorkGroupName = i.WorkGroup.GroupName,
                        Description = i.Description,
                        UnitOfMeasure = i.UnitOfMeasure,
                        Quantity = i.Quantity,
                        Rate = i.Rate
                    }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);
        if (model is null) return null;

        var company = await _unitOfWork.Repository<Company>().Query().AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new { x.Name, x.Address, x.Phone, x.Email })
            .FirstOrDefaultAsync(cancellationToken);
        model.CompanyName = company?.Name ?? string.Empty;
        model.CompanyAddress = company?.Address;
        model.CompanyPhone = company?.Phone;
        model.CompanyEmail = company?.Email;
        return model;
    }
}
