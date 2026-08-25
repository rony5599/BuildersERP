using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalNotices;

public record CreateLegalNoticeCommand(CreateLegalNoticeDto Dto) : IRequest<long>;

public class CreateLegalNoticeCommandHandler : IRequestHandler<CreateLegalNoticeCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLegalNoticeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateLegalNoticeCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<LegalNotice>(request.Dto);
        var repository = _unitOfWork.Repository<LegalNotice>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
