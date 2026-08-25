using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.TestReports;

public record CreateTestReportCommand(CreateTestReportDto Dto) : IRequest<long>;

public class CreateTestReportCommandHandler : IRequestHandler<CreateTestReportCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTestReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateTestReportCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<TestReport>(request.Dto);
        await _unitOfWork.Repository<TestReport>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
