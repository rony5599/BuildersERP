using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Brokers;

public record CreateBrokerCommand(CreateBrokerDto Dto) : IRequest<long>;

public class CreateBrokerCommandHandler : IRequestHandler<CreateBrokerCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBrokerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateBrokerCommand request, CancellationToken cancellationToken)
    {
        var broker = _mapper.Map<Broker>(request.Dto);
        await _unitOfWork.Repository<Broker>().AddAsync(broker);
        await _unitOfWork.SaveChangesAsync();

        return broker.Id;
    }
}
