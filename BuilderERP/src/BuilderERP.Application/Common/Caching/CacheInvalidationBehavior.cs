using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuilderERP.Application.Common.Caching;

/// <summary>
/// After a command (any request whose type name ends with "Command") completes
/// successfully, bumps that feature's cache version so every query cached under
/// <see cref="CachingBehavior{TRequest,TResponse}"/> for the same feature is
/// invalidated on the next read.
/// </summary>
public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationBehavior(IDistributedCache cache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        var requestType = typeof(TRequest);
        if (requestType.Name.EndsWith("Command", StringComparison.Ordinal))
        {
            var feature = CacheKeys.GetFeatureName(requestType);
            try
            {
                await CacheKeys.BumpFeatureVersionAsync(_cache, feature, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache invalidation failed for feature {Feature}.", feature);
            }
        }

        return response;
    }
}
