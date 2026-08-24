using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Rfqs;

public record CreateRfqCommand(CreateRfqDto Dto) : IRequest<Guid>;

public class CreateRfqCommandHandler : IRequestHandler<CreateRfqCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRfqCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateRfqCommand request, CancellationToken cancellationToken)
    {
        var rfq = _mapper.Map<Rfq>(request.Dto);

        foreach (var supplierId in request.Dto.SupplierIds.Distinct())
        {
            rfq.RfqVendors.Add(new RfqVendor { RfqId = rfq.Id, SupplierId = supplierId });
        }

        foreach (var detail in request.Dto.Details)
        {
            rfq.Details.Add(new RfqDetail
            {
                RfqId = rfq.Id,
                MaterialId = detail.MaterialId,
                Quantity = detail.Quantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                Specification = detail.Specification
            });
        }

        await _unitOfWork.Repository<Rfq>().AddAsync(rfq);
        await _unitOfWork.SaveChangesAsync();

        return rfq.Id;
    }
}
