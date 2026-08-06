using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Receipts;

public record GetReceiptByIdQuery(Guid Id) : IRequest<ReceiptDto?>;

public class GetReceiptByIdQueryHandler : IRequestHandler<GetReceiptByIdQuery, ReceiptDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReceiptByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReceiptDto?> Handle(GetReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var receipt = await _unitOfWork.Repository<Receipt>().GetByIdAsync(request.Id);
        return receipt is null ? null : _mapper.Map<ReceiptDto>(receipt);
    }
}
