using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Quotations;

public record CreateQuotationCommand(CreateQuotationDto Dto) : IRequest<long>;

public class CreateQuotationCommandHandler : IRequestHandler<CreateQuotationCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateQuotationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateQuotationCommand request, CancellationToken cancellationToken)
    {
        var quotation = _mapper.Map<Quotation>(request.Dto);
        await _unitOfWork.Repository<Quotation>().AddAsync(quotation);
        await _unitOfWork.SaveChangesAsync();

        return quotation.Id;
    }
}
