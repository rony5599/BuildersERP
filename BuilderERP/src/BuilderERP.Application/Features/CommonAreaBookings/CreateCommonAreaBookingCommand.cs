using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CommonAreaBookings;

public record CreateCommonAreaBookingCommand(CreateCommonAreaBookingDto Dto) : IRequest<long>;

public class CreateCommonAreaBookingCommandHandler : IRequestHandler<CreateCommonAreaBookingCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCommonAreaBookingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateCommonAreaBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<CommonAreaBooking>(request.Dto);
        var repository = _unitOfWork.Repository<CommonAreaBooking>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
