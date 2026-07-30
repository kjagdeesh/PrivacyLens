using PrivacyLens.Data.Entities;
using PrivacyLens.Models;
using System;

namespace PrivacyLens.Data.Mappers
{
    public static class DevicePermissionEntityMapper
    {
        public static DevicePermission ToDomain(this DevicePermissionEntity entity)
        {
            return new DevicePermission
            {
                PermissionName = entity.PermissionName,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Category = entity.PermissionCategory,
                GrantedAppCount = entity.GrantedAppCount,
                IsSupported = entity.IsSupported
            };
        }

        public static DevicePermissionEntity ToEntity(this DevicePermission domain)
        {
            return new DevicePermissionEntity
            {
                PermissionName = domain.PermissionName,
                DisplayName = domain.DisplayName,
                Description = domain.Description,
                PermissionCategory = domain.Category,
                GrantedAppCount = domain.GrantedAppCount,
                IsSupported = domain.IsSupported,
                LastUpdatedAt = DateTime.UtcNow
            };
        }
    }
}
