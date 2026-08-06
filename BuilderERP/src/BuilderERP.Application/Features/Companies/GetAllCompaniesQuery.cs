using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Companies;

public record GetAllCompaniesQuery : IRequest<IReadOnlyList<CompanyDto>>;

public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, IReadOnlyList<CompanyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCompaniesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CompanyDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _unitOfWork.Repository<Company>().GetAllAsync();
        return _mapper.Map<IReadOnlyList<CompanyDto>>(companies);
    }
}
