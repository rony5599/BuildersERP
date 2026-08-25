using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CustomerCommunications;

public record GetCustomerCommunicationByIdQuery(long Id) : IRequest<CustomerCommunicationDto?>;

public class GetCustomerCommunicationByIdQueryHandler : IRequestHandler<GetCustomerCommunicationByIdQuery, CustomerCommunicationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomerCommunicationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerCommunicationDto?> Handle(GetCustomerCommunicationByIdQuery request, CancellationToken cancellationToken)
    {
        var communication = await _unitOfWork.Repository<CustomerCommunication>().GetByIdAsync(request.Id);
        return communication is null ? null : _mapper.Map<CustomerCommunicationDto>(communication);
    }
}
