using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CustomerCommunications;

public record CreateCustomerCommunicationCommand(CreateCustomerCommunicationDto Dto) : IRequest<Guid>;

public class CreateCustomerCommunicationCommandHandler : IRequestHandler<CreateCustomerCommunicationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCustomerCommunicationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateCustomerCommunicationCommand request, CancellationToken cancellationToken)
    {
        var communication = _mapper.Map<CustomerCommunication>(request.Dto);
        await _unitOfWork.Repository<CustomerCommunication>().AddAsync(communication);
        await _unitOfWork.SaveChangesAsync();

        return communication.Id;
    }
}
