namespace BuilderERP.Domain.Interfaces;

public interface IDocumentNumberGenerator
{
    Task<string> GenerateAsync(Guid projectId, string documentType, CancellationToken cancellationToken = default);
}
