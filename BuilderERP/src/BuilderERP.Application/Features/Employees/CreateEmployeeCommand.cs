using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Employees;

public record CreateEmployeeCommand(CreateEmployeeDto Dto) : IRequest<long>;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEmployeeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = _mapper.Map<Employee>(request.Dto);
        await _unitOfWork.Repository<Employee>().AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return employee.Id;
    }
}
