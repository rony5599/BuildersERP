using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FlatHandovers;

public record CreateFlatHandoverCommand(CreateFlatHandoverDto Dto) : IRequest<long>;

public class CreateFlatHandoverCommandHandler : IRequestHandler<CreateFlatHandoverCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateFlatHandoverCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateFlatHandoverCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<FlatHandover>(request.Dto);
        var repository = _unitOfWork.Repository<FlatHandover>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
