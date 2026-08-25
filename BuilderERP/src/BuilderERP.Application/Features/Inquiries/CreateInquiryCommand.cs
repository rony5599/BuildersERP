using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Inquiries;

public record CreateInquiryCommand(CreateInquiryDto Dto) : IRequest<long>;

public class CreateInquiryCommandHandler : IRequestHandler<CreateInquiryCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInquiryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateInquiryCommand request, CancellationToken cancellationToken)
    {
        var inquiry = _mapper.Map<Inquiry>(request.Dto);
        await _unitOfWork.Repository<Inquiry>().AddAsync(inquiry);
        await _unitOfWork.SaveChangesAsync();

        return inquiry.Id;
    }
}
