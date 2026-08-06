using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Rfqs;

public record GetRfqByIdQuery(Guid Id) : IRequest<RfqDto?>;

public class GetRfqByIdQueryHandler : IRequestHandler<GetRfqByIdQuery, RfqDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRfqByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RfqDto?> Handle(GetRfqByIdQuery request, CancellationToken cancellationToken)
    {
        var rfq = await _unitOfWork.Repository<Rfq>().GetByIdAsync(request.Id);
        return rfq is null ? null : _mapper.Map<RfqDto>(rfq);
    }
}
