using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CustomerCommunications;

public record GetAllCustomerCommunicationsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<CustomerCommunicationDto>>;

public class GetAllCustomerCommunicationsQueryHandler : IRequestHandler<GetAllCustomerCommunicationsQuery, PagedResult<CustomerCommunicationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCustomerCommunicationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CustomerCommunicationDto>> Handle(GetAllCustomerCommunicationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<CustomerCommunication>().Query()
            .Include(c => c.Customer)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var communications = await query
            .OrderByDescending(c => c.CommunicationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<CustomerCommunicationDto>>(communications);
        return new PagedResult<CustomerCommunicationDto>(items, totalCount, page, pageSize);
    }
}
