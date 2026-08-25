using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LegalNotices;

public record GetLegalNoticeByIdQuery(long Id) : IRequest<LegalNoticeDto?>;

public class GetLegalNoticeByIdQueryHandler : IRequestHandler<GetLegalNoticeByIdQuery, LegalNoticeDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLegalNoticeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LegalNoticeDto?> Handle(GetLegalNoticeByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<LegalNotice>().Query()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<LegalNoticeDto>(item);
    }
}
