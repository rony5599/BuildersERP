using BuilderERP.Domain.Interfaces;
using BuilderERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Infrastructure.Repositories;

public class DocumentNumberGenerator : IDocumentNumberGenerator
{
    private readonly AppDbContext _context;

    public DocumentNumberGenerator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync(Guid projectId, string documentType, CancellationToken cancellationToken = default)
    {
        var projectCode = await _context.Projects
            .Where(p => p.Id == projectId)
            .Select(p => p.Code)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"Project '{projectId}' was not found.");

        // MERGE ... WITH (HOLDLOCK) avoids the classic MERGE race condition where two
        // concurrent inserts for the same key both take the "not matched" branch.
        // A MERGE statement isn't composable SQL, so the result must be materialized
        // with ToListAsync rather than SingleAsync (which would make EF wrap it in a subquery).
        var results = await _context.Database.SqlQuery<int>($@"
MERGE INTO DocumentSequences WITH (HOLDLOCK) AS target
USING (SELECT {projectId} AS ProjectId, {documentType} AS DocumentType) AS src
ON target.ProjectId = src.ProjectId AND target.DocumentType = src.DocumentType
WHEN MATCHED THEN UPDATE SET target.LastNumber = target.LastNumber + 1
WHEN NOT MATCHED THEN INSERT (Id, ProjectId, DocumentType, LastNumber) VALUES (NEWID(), src.ProjectId, src.DocumentType, 1)
OUTPUT INSERTED.LastNumber;")
            .ToListAsync(cancellationToken);
        var nextNumber = results.Single();

        return $"{projectCode}-{documentType}-{nextNumber:D4}";
    }
}
