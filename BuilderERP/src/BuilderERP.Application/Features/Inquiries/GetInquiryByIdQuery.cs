using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Inquiries;

public record GetInquiryByIdQuery(long Id) : IRequest<InquiryDto?>;

public class GetInquiryByIdQueryHandler : IRequestHandler<GetInquiryByIdQuery, InquiryDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetInquiryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InquiryDto?> Handle(GetInquiryByIdQuery request, CancellationToken cancellationToken)
    {
        var inquiry = await _unitOfWork.Repository<Inquiry>().GetByIdAsync(request.Id);
        return inquiry is null ? null : _mapper.Map<InquiryDto>(inquiry);
    }
}
