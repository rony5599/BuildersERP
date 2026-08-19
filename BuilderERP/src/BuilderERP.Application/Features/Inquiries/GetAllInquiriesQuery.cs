using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Inquiries;

public record GetAllInquiriesQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<InquiryDto>>;

public class GetAllInquiriesQueryHandler : IRequestHandler<GetAllInquiriesQuery, PagedResult<InquiryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllInquiriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<InquiryDto>> Handle(GetAllInquiriesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Inquiry>().Query()
            .Include(i => i.Lead)
            .Include(i => i.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var inquiries = await query
            .OrderByDescending(i => i.InquiryDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<InquiryDto>>(inquiries);
        return new PagedResult<InquiryDto>(items, totalCount, page, pageSize);
    }
}
