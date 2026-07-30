using PrivacyLens.Data.Entities;
using PrivacyLens.Models;
using System;

namespace PrivacyLens.Data.Mappers
{
    public static class PermissionCapabilityEntityMapper
    {
        public static PermissionCapability ToDomain(this PermissionCapabilityEntity entity)
        {
            return new PermissionCapability
            {
                Category = entity.PermissionCategory,
                IsSupported = entity.IsSupported,
                CanReadGrantedStatus = entity.CanReadGrantedStatus,
                CanReadLastUsage = entity.CanReadLastUsage,
                RequiresSpecialAccess = entity.RequiresSpecialAccess,
                LimitationDescription = entity.LimitationDescription
            };
        }

        public static PermissionCapabilityEntity ToEntity(this PermissionCapability domain)
        {
            return new PermissionCapabilityEntity
            {
                PermissionCategory = domain.Category,
                IsSupported = domain.IsSupported,
                CanReadGrantedStatus = domain.CanReadGrantedStatus,
                CanReadLastUsage = domain.CanReadLastUsage,
                RequiresSpecialAccess = domain.RequiresSpecialAccess,
                LimitationDescription = domain.LimitationDescription,
                LastUpdatedAt = DateTime.UtcNow
            };
        }
    }
}
