using PrivacyLens.Data.Entities;
using PrivacyLens.Models;
using System;

namespace PrivacyLens.Data.Mappers
{
    public static class AppPermissionEntityMapper
    {
        public static AppPermission ToDomain(this AppPermissionEntity entity)
        {
            return new AppPermission
            {
                PermissionName = entity.PermissionName,
                DisplayName = entity.DisplayName,
                Category = entity.PermissionCategory,
                Status = entity.PermissionAccessStatus,
                IsGranted = entity.IsGranted,
                LastAccessTime = entity.LastAccessTime.HasValue 
                    ? new DateTimeOffset(entity.LastAccessTime.Value, TimeSpan.Zero) 
                    : null,
                UsageDataAvailability = entity.UsageDataAvailability
            };
        }

        public static AppPermissionEntity ToEntity(this AppPermission domain, string packageName)
        {
            return new AppPermissionEntity
            {
                PackageName = packageName,
                PermissionName = domain.PermissionName,
                DisplayName = domain.DisplayName,
                PermissionCategory = domain.Category,
                PermissionAccessStatus = domain.Status,
                IsGranted = domain.IsGranted,
                LastAccessTime = domain.LastAccessTime?.UtcDateTime,
                UsageDataAvailability = domain.UsageDataAvailability,
                LastUpdatedAt = DateTime.UtcNow
            };
        }
    }
}
