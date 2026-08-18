using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BuilderERP.Application.Common.Caching;

/// <summary>
/// Builds Redis cache keys grouped by feature (e.g. "Customers", "RunningBills").
/// Each feature has a version counter; commands bump it so every cached query
/// under that feature is invalidated at once, without tracking individual keys.
/// </summary>
internal static class CacheKeys
{
    private const string Prefix = "BuilderERP";
    private const string FeaturesMarker = ".Features.";

    public static string GetFeatureName(Type requestType)
    {
        var ns = requestType.Namespace ?? string.Empty;
        var idx = ns.IndexOf(FeaturesMarker, StringComparison.Ordinal);
        if (idx < 0)
        {
            return ns;
        }

        var rest = ns[(idx + FeaturesMarker.Length)..];
        var dot = rest.IndexOf('.');
        return dot < 0 ? rest : rest[..dot];
    }

    public static async Task<long> GetFeatureVersionAsync(IDistributedCache cache, string feature, CancellationToken cancellationToken)
    {
        var raw = await cache.GetStringAsync(VersionKey(feature), cancellationToken);
        return raw is not null && long.TryParse(raw, out var version) ? version : 1L;
    }

    public static async Task BumpFeatureVersionAsync(IDistributedCache cache, string feature, CancellationToken cancellationToken)
    {
        var current = await GetFeatureVersionAsync(cache, feature, cancellationToken);
        await cache.SetStringAsync(VersionKey(feature), (current + 1).ToString(), cancellationToken);
    }

    public static string BuildQueryKey(string feature, long version, string requestTypeName, object request)
    {
        var payload = JsonSerializer.Serialize(request, request.GetType());
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));
        return $"{Prefix}:{feature}:v{version}:{requestTypeName}:{hash}";
    }

    private static string VersionKey(string feature) => $"{Prefix}:{feature}:version";
}
