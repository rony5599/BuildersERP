using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuilderERP.Application.Common.Caching;

/// <summary>
/// Transparently caches the result of every MediatR query (any request whose type
/// name ends with "Query") in Redis (or the in-memory fallback), so list/detail
/// pages served repeatedly don't re-hit the database. Commands are left untouched
/// here; <see cref="CacheInvalidationBehavior{TRequest,TResponse}"/> evicts the
/// affected feature's cache after a command runs.
/// </summary>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        if (!requestType.Name.EndsWith("Query", StringComparison.Ordinal))
        {
            return await next();
        }

        var feature = CacheKeys.GetFeatureName(requestType);
        string? cacheKey = null;

        try
        {
            var version = await CacheKeys.GetFeatureVersionAsync(_cache, feature, cancellationToken);
            cacheKey = CacheKeys.BuildQueryKey(feature, version, requestType.Name, request);

            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (cached is not null)
            {
                var deserialized = JsonSerializer.Deserialize<TResponse>(cached);
                if (deserialized is not null)
                {
                    return deserialized;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache read failed for {RequestType}; bypassing cache.", requestType.Name);
        }

        var response = await next();

        if (cacheKey is not null)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(response);
                await _cache.SetStringAsync(
                    cacheKey,
                    serialized,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = DefaultExpiration },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache write failed for {RequestType}.", requestType.Name);
            }
        }

        return response;
    }
}
