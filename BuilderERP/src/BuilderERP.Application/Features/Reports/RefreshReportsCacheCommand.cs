using MediatR;

namespace BuilderERP.Application.Features.Reports;

/// <summary>
/// Bumps the Reports cache version through CacheInvalidationBehavior so the next
/// report request is read from the database instead of Redis.
/// </summary>
public record RefreshReportsCacheCommand : IRequest;

public class RefreshReportsCacheCommandHandler : IRequestHandler<RefreshReportsCacheCommand>
{
    public Task Handle(RefreshReportsCacheCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
