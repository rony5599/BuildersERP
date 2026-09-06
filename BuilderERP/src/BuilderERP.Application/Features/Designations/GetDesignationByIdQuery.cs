using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Designations;

public record GetDesignationByIdQuery(long Id) : IRequest<DesignationDto?>;

public class GetDesignationByIdQueryHandler : IRequestHandler<GetDesignationByIdQuery, DesignationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDesignationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DesignationDto?> Handle(GetDesignationByIdQuery request, CancellationToken cancellationToken)
    {
        var designation = await _unitOfWork.Repository<Designation>().GetByIdAsync(request.Id);
        return designation is null ? null : _mapper.Map<DesignationDto>(designation);
    }
}
