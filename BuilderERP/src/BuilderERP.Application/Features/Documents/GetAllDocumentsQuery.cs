using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Documents;

public record GetAllDocumentsQuery(Guid? CustomerId = null, Guid? ProjectId = null) : IRequest<IReadOnlyList<DocumentDto>>;

public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, IReadOnlyList<DocumentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDocumentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Document>().Query()
            .Include(x => x.Customer)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == request.CustomerId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query.OrderBy(x => x.DocumentNumber).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DocumentDto>>(items);
    }
}
