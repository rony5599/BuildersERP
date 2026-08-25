namespace BuilderERP.Domain.Interfaces;

public interface IDocumentNumberGenerator
{
    Task<string> GenerateAsync(long projectId, string documentType, CancellationToken cancellationToken = default);
}
