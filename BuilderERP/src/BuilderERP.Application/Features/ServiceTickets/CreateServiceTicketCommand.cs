using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ServiceTickets;

public record CreateServiceTicketCommand(CreateServiceTicketDto Dto) : IRequest<long>;

public class CreateServiceTicketCommandHandler : IRequestHandler<CreateServiceTicketCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateServiceTicketCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateServiceTicketCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ServiceTicket>(request.Dto);
        var repository = _unitOfWork.Repository<ServiceTicket>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
