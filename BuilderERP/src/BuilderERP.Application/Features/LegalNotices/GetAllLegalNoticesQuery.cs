using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.LegalNotices;

public record GetAllLegalNoticesQuery : IRequest<IReadOnlyList<LegalNoticeDto>>;

public class GetAllLegalNoticesQueryHandler : IRequestHandler<GetAllLegalNoticesQuery, IReadOnlyList<LegalNoticeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLegalNoticesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LegalNoticeDto>> Handle(GetAllLegalNoticesQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<LegalNotice>().Query()
            .Include(x => x.Project)
            .OrderBy(x => x.NoticeNumber)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LegalNoticeDto>>(items);
    }
}
