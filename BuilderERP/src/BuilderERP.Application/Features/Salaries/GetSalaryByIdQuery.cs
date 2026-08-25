using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Salaries;

public record GetSalaryByIdQuery(long Id) : IRequest<SalaryDto?>;

public class GetSalaryByIdQueryHandler : IRequestHandler<GetSalaryByIdQuery, SalaryDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSalaryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SalaryDto?> Handle(GetSalaryByIdQuery request, CancellationToken cancellationToken)
    {
        var salary = await _unitOfWork.Repository<Salary>().GetByIdAsync(request.Id);
        return salary is null ? null : _mapper.Map<SalaryDto>(salary);
    }
}
