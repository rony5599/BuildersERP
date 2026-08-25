using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Salaries;

public record CreateSalaryCommand(CreateSalaryDto Dto) : IRequest<long>;

public class CreateSalaryCommandHandler : IRequestHandler<CreateSalaryCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSalaryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateSalaryCommand request, CancellationToken cancellationToken)
    {
        var salary = _mapper.Map<Salary>(request.Dto);
        salary.NetAmount = salary.BasicAmount + salary.OvertimeAmount - salary.DeductionAmount;
        await _unitOfWork.Repository<Salary>().AddAsync(salary);
        await _unitOfWork.SaveChangesAsync();
        return salary.Id;
    }
}
