using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CommonAreaBookings;

public record GetCommonAreaBookingByIdQuery(Guid Id) : IRequest<CommonAreaBookingDto?>;

public class GetCommonAreaBookingByIdQueryHandler : IRequestHandler<GetCommonAreaBookingByIdQuery, CommonAreaBookingDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCommonAreaBookingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CommonAreaBookingDto?> Handle(GetCommonAreaBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<CommonAreaBooking>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<CommonAreaBookingDto>(item);
    }
}
