using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DailyProgresses;

public record GetDailyProgressByIdQuery(Guid Id) : IRequest<DailyProgressDto?>;

public class GetDailyProgressByIdQueryHandler : IRequestHandler<GetDailyProgressByIdQuery, DailyProgressDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDailyProgressByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DailyProgressDto?> Handle(GetDailyProgressByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<DailyProgress>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<DailyProgressDto>(item);
    }
}
