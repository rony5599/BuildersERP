using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CustomerCommunications;

public record GetAllCustomerCommunicationsQuery : IRequest<IReadOnlyList<CustomerCommunicationDto>>;

public class GetAllCustomerCommunicationsQueryHandler : IRequestHandler<GetAllCustomerCommunicationsQuery, IReadOnlyList<CustomerCommunicationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCustomerCommunicationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CustomerCommunicationDto>> Handle(GetAllCustomerCommunicationsQuery request, CancellationToken cancellationToken)
    {
        var communications = await _unitOfWork.Repository<CustomerCommunication>().Query()
            .Include(c => c.Customer)
            .OrderByDescending(c => c.CommunicationDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CustomerCommunicationDto>>(communications);
    }
}
