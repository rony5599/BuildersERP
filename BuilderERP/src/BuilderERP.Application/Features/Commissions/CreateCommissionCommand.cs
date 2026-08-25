using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Commissions;

public record CreateCommissionCommand(CreateCommissionDto Dto) : IRequest<long>;

public class CreateCommissionCommandHandler : IRequestHandler<CreateCommissionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCommissionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
    {
        var commission = _mapper.Map<Commission>(request.Dto);
        commission.CommissionAmount = await CommissionCalculator.ResolveAmountAsync(_unitOfWork, request.Dto.BookingId, request.Dto.CommissionRate, request.Dto.CommissionAmount);

        await _unitOfWork.Repository<Commission>().AddAsync(commission);
        await _unitOfWork.SaveChangesAsync();

        return commission.Id;
    }
}

internal static class CommissionCalculator
{
    public static async Task<decimal> ResolveAmountAsync(IUnitOfWork unitOfWork, long bookingId, decimal commissionRate, decimal? explicitAmount)
    {
        if (explicitAmount.HasValue)
        {
            return explicitAmount.Value;
        }

        var booking = await unitOfWork.Repository<Booking>().GetByIdAsync(bookingId);
        if (booking is null)
        {
            return 0m;
        }

        return Math.Round(booking.BookingAmount * commissionRate / 100m, 2);
    }
}
