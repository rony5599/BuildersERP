using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.TestReports;

public record UpdateTestReportCommand(UpdateTestReportDto Dto) : IRequest<bool>;

public class UpdateTestReportCommandHandler : IRequestHandler<UpdateTestReportCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTestReportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateTestReportCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<TestReport>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.ReportNumber = request.Dto.ReportNumber;
        item.TestType = request.Dto.TestType;
        item.TestDate = request.Dto.TestDate;
        item.LabName = request.Dto.LabName;
        item.Result = request.Dto.Result;
        item.FilePath = request.Dto.FilePath;
        item.ProjectId = request.Dto.ProjectId;
        item.MaterialId = request.Dto.MaterialId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
