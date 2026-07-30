using PrivacyLens.Data.Entities;
using PrivacyLens.Models;
using System;

namespace PrivacyLens.Data.Mappers
{
    public static class PermissionUsageEntityMapper
    {
        public static PermissionUsageRecord ToDomain(this PermissionUsageEntity entity)
        {
            return new PermissionUsageRecord
            {
                PackageName = entity.PackageName,
                AppName = entity.AppName,
                Category = entity.PermissionCategory,
                PermissionName = entity.PermissionName,
                AccessTime = entity.AccessTime.HasValue 
                    ? new DateTimeOffset(entity.AccessTime.Value, TimeSpan.Zero) 
                    : null,
                IsBackgroundAccess = entity.IsBackgroundAccess,
                IsCurrentlyActive = entity.IsCurrentlyActive,
                DataAvailability = entity.DataAvailability
            };
        }

        public static PermissionUsageEntity ToEntity(this PermissionUsageRecord domain)
        {
            return new PermissionUsageEntity
            {
                PackageName = domain.PackageName,
                AppName = domain.AppName,
                PermissionCategory = domain.Category,
                PermissionName = domain.PermissionName,
                AccessTime = domain.AccessTime?.UtcDateTime,
                IsBackgroundAccess = domain.IsBackgroundAccess,
                IsCurrentlyActive = domain.IsCurrentlyActive,
                DataAvailability = domain.DataAvailability,
                DetectedAt = DateTime.UtcNow
            };
        }
    }
}
