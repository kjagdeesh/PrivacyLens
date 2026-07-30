using PrivacyLens.Data.Entities;
using PrivacyLens.Models;
using System;

namespace PrivacyLens.Data.Mappers
{
    public static class InstalledAppEntityMapper
    {
        public static InstalledApp ToDomain(this InstalledAppEntity entity)
        {
            return new InstalledApp
            {
                PackageName = entity.PackageName,
                Name = entity.Name,
                VersionName = entity.VersionName,
                VersionCode = entity.VersionCode,
                IsSystemApp = entity.IsSystemApp,
                IconCachePath = entity.IconCachePath,
                AppProvidedDescription = entity.AppProvidedDescription,
                GrantedSensitivePermissionCount = entity.GrantedSensitivePermissionCount
            };
        }

        public static InstalledAppEntity ToEntity(this InstalledApp domain, DateTime firstDetected, DateTime lastDetected)
        {
            return new InstalledAppEntity
            {
                PackageName = domain.PackageName,
                Name = domain.Name,
                VersionName = domain.VersionName,
                VersionCode = domain.VersionCode,
                IsSystemApp = domain.IsSystemApp,
                IconCachePath = domain.IconCachePath,
                GrantedSensitivePermissionCount = domain.GrantedSensitivePermissionCount,
                FirstDetectedAt = firstDetected,
                LastDetectedAt = lastDetected,
                LastUpdatedAt = DateTime.UtcNow
            };
        }
    }
}
