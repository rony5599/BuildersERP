using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Designations;

public record CreateDesignationCommand(CreateDesignationDto Dto) : IRequest<long>;

public class CreateDesignationCommandHandler : IRequestHandler<CreateDesignationCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDesignationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateDesignationCommand request, CancellationToken cancellationToken)
    {
        var designation = _mapper.Map<Designation>(request.Dto);
        await _unitOfWork.Repository<Designation>().AddAsync(designation);
        await _unitOfWork.SaveChangesAsync();

        return designation.Id;
    }
}
