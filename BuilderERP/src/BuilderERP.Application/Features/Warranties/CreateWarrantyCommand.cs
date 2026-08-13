using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Warranties;

public record CreateWarrantyCommand(CreateWarrantyDto Dto) : IRequest<Guid>;

public class CreateWarrantyCommandHandler : IRequestHandler<CreateWarrantyCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateWarrantyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateWarrantyCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Warranty>(request.Dto);
        var repository = _unitOfWork.Repository<Warranty>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
