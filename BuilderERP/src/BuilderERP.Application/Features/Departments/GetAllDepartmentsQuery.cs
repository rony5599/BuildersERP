using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Departments;

public record GetAllDepartmentsQuery : IRequest<IReadOnlyList<DepartmentDto>>;

public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, IReadOnlyList<DepartmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _unitOfWork.Repository<Department>().Query().Include(d => d.Branch).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DepartmentDto>>(departments);
    }
}
