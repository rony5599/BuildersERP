using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PerformanceEvaluations;

public record GetPerformanceEvaluationByIdQuery(Guid Id) : IRequest<PerformanceEvaluationDto?>;

public class GetPerformanceEvaluationByIdQueryHandler : IRequestHandler<GetPerformanceEvaluationByIdQuery, PerformanceEvaluationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPerformanceEvaluationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PerformanceEvaluationDto?> Handle(GetPerformanceEvaluationByIdQuery request, CancellationToken cancellationToken)
    {
        var evaluation = await _unitOfWork.Repository<PerformanceEvaluation>().GetByIdAsync(request.Id);
        return evaluation is null ? null : _mapper.Map<PerformanceEvaluationDto>(evaluation);
    }
}
