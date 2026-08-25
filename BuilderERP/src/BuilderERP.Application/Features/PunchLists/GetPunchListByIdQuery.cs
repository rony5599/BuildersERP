using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PunchLists;

public record GetPunchListByIdQuery(long Id) : IRequest<PunchListDto?>;

public class GetPunchListByIdQueryHandler : IRequestHandler<GetPunchListByIdQuery, PunchListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPunchListByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PunchListDto?> Handle(GetPunchListByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<PunchList>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<PunchListDto>(item);
    }
}
