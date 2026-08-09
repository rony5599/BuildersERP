using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Ncrs;

public record GetAllNcrsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<NcrDto>>;

public class GetAllNcrsQueryHandler : IRequestHandler<GetAllNcrsQuery, IReadOnlyList<NcrDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllNcrsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<NcrDto>> Handle(GetAllNcrsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Ncr>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query.OrderByDescending(x => x.RaisedDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<NcrDto>>(items);
    }
}
