using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PerformanceEvaluations;

public record CreatePerformanceEvaluationCommand(CreatePerformanceEvaluationDto Dto) : IRequest<long>;

public class CreatePerformanceEvaluationCommandHandler : IRequestHandler<CreatePerformanceEvaluationCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePerformanceEvaluationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreatePerformanceEvaluationCommand request, CancellationToken cancellationToken)
    {
        var evaluation = _mapper.Map<PerformanceEvaluation>(request.Dto);
        await _unitOfWork.Repository<PerformanceEvaluation>().AddAsync(evaluation);
        await _unitOfWork.SaveChangesAsync();
        return evaluation.Id;
    }
}
