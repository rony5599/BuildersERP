using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Mappings;

public class DocumentVersionMappingProfile : Profile
{
    public DocumentVersionMappingProfile()
    {
        CreateMap<DocumentVersion, DocumentVersionDto>()
            .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.Document != null ? src.Document.DocumentNumber : string.Empty));
        CreateMap<CreateDocumentVersionDto, DocumentVersion>();
        CreateMap<UpdateDocumentVersionDto, DocumentVersion>();
    }
}
