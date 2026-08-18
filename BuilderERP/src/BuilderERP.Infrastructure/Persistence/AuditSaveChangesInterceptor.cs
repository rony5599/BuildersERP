using BuilderERP.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuilderERP.Infrastructure.Persistence;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    // Bookkeeping columns already surfaced on every entity via CreatedAt/CreatedBy/ModifiedAt/ModifiedBy;
    // diffing them as well would double up every audited change with noise.
    private static readonly HashSet<string> ExcludedProperties = new()
    {
        nameof(BaseEntity.CreatedAt), nameof(BaseEntity.CreatedBy),
        nameof(BaseEntity.ModifiedAt), nameof(BaseEntity.ModifiedBy)
    };

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditInfo(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        var now = DateTime.UtcNow;
        var auditEntries = new List<AuditLog>();

        // AuditLog itself does not derive from BaseEntity, so it never appears in this loop
        // and audit rows can't recursively audit themselves.
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userName;
                auditEntries.Add(BuildCreatedLog(entry, userName, now));
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = now;
                entry.Entity.ModifiedBy = userName;
                auditEntries.AddRange(BuildModifiedLogs(entry, userName, now));
            }
            else if (entry.State == EntityState.Deleted)
            {
                auditEntries.Add(BuildDeletedLog(entry, userName, now));
            }
        }

        if (auditEntries.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditEntries);
        }
    }

    private static AuditLog BuildCreatedLog(EntityEntry<BaseEntity> entry, string? userName, DateTime now)
    {
        return new AuditLog
        {
            EntityName = entry.Entity.GetType().Name,
            EntityId = entry.Entity.Id,
            Action = AuditAction.Created,
            OldValue = null,
            NewValue = FormatValues(entry.CurrentValues),
            ChangedBy = userName,
            ChangedAt = now
        };
    }

    private static AuditLog BuildDeletedLog(EntityEntry<BaseEntity> entry, string? userName, DateTime now)
    {
        return new AuditLog
        {
            EntityName = entry.Entity.GetType().Name,
            EntityId = entry.Entity.Id,
            Action = AuditAction.Deleted,
            OldValue = FormatValues(entry.OriginalValues),
            NewValue = null,
            ChangedBy = userName,
            ChangedAt = now
        };
    }

    private static IEnumerable<AuditLog> BuildModifiedLogs(EntityEntry<BaseEntity> entry, string? userName, DateTime now)
    {
        var entityName = entry.Entity.GetType().Name;

        // A soft-delete (IsDeleted flipping to true) is recorded as a single Deleted entry
        // rather than a per-field Modified diff, so the audit trail reads the same way for
        // soft and hard deletes.
        var isDeletedProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == nameof(BaseEntity.IsDeleted));
        if (isDeletedProperty is { IsModified: true } && Equals(isDeletedProperty.CurrentValue, true))
        {
            yield return new AuditLog
            {
                EntityName = entityName,
                EntityId = entry.Entity.Id,
                Action = AuditAction.Deleted,
                OldValue = FormatValues(entry.OriginalValues),
                NewValue = null,
                ChangedBy = userName,
                ChangedAt = now
            };
            yield break;
        }

        foreach (var property in entry.Properties)
        {
            if (!property.IsModified || ExcludedProperties.Contains(property.Metadata.Name))
            {
                continue;
            }

            var oldValue = property.OriginalValue;
            var newValue = property.CurrentValue;
            if (Equals(oldValue, newValue))
            {
                continue;
            }

            yield return new AuditLog
            {
                EntityName = entityName,
                EntityId = entry.Entity.Id,
                Action = AuditAction.Modified,
                PropertyName = property.Metadata.Name,
                OldValue = FormatValue(oldValue),
                NewValue = FormatValue(newValue),
                ChangedBy = userName,
                ChangedAt = now
            };
        }
    }

    private static string? FormatValue(object? value) => value?.ToString();

    private static string FormatValues(PropertyValues values)
    {
        var parts = values.Properties
            .Where(p => !ExcludedProperties.Contains(p.Name))
            .Select(p => $"{p.Name}={FormatValue(values[p]) ?? "null"}");
        return string.Join("; ", parts);
    }
}
