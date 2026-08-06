using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Projects;

public record GetAllProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllProjectsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _unitOfWork.Repository<Project>().Query().Include(p => p.Branch).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ProjectDto>>(projects);
    }
}
