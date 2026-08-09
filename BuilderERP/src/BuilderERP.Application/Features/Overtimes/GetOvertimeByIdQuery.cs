using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Overtimes;

public record GetOvertimeByIdQuery(Guid Id) : IRequest<OvertimeDto?>;

public class GetOvertimeByIdQueryHandler : IRequestHandler<GetOvertimeByIdQuery, OvertimeDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOvertimeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OvertimeDto?> Handle(GetOvertimeByIdQuery request, CancellationToken cancellationToken)
    {
        var overtime = await _unitOfWork.Repository<Overtime>().GetByIdAsync(request.Id);
        return overtime is null ? null : _mapper.Map<OvertimeDto>(overtime);
    }
}
