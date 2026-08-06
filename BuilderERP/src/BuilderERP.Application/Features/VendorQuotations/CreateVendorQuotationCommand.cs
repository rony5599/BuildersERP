using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.VendorQuotations;

public record CreateVendorQuotationCommand(CreateVendorQuotationDto Dto) : IRequest<Guid>;

public class CreateVendorQuotationCommandHandler : IRequestHandler<CreateVendorQuotationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateVendorQuotationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateVendorQuotationCommand request, CancellationToken cancellationToken)
    {
        var quotation = _mapper.Map<VendorQuotation>(request.Dto);
        await _unitOfWork.Repository<VendorQuotation>().AddAsync(quotation);
        await _unitOfWork.SaveChangesAsync();
        return quotation.Id;
    }
}
