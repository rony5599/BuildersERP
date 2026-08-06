using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Inquiries;

public record GetAllInquiriesQuery : IRequest<IReadOnlyList<InquiryDto>>;

public class GetAllInquiriesQueryHandler : IRequestHandler<GetAllInquiriesQuery, IReadOnlyList<InquiryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInquiriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InquiryDto>> Handle(GetAllInquiriesQuery request, CancellationToken cancellationToken)
    {
        var inquiries = await _unitOfWork.Repository<Inquiry>().Query()
            .Include(i => i.Lead)
            .Include(i => i.PropertyUnit)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<InquiryDto>>(inquiries);
    }
}
