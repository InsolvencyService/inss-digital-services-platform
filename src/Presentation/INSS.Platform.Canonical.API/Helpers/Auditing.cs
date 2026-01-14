using INSS.Platform.Canonical.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Canonical.API.Helpers;

/// <summary>
/// Provides helper methods for setting audit information on <see cref="BaseEntity"/> instances.
/// </summary>
internal static class Auditing
{
    /// <summary>
    /// Sets the creation audit information on the specified <see cref="BaseEntity"/>.
    /// </summary>
    /// <param name="entity">The entity to set creation information for.</param>
    /// <param name="controllerContext">The controller context used to retrieve the current user.</param>
    internal static void SetCreatedInfo(BaseEntity entity, ControllerContext controllerContext)
    {
        entity.Created ??= DateTime.UtcNow;
        entity.CreatedBy = string.IsNullOrWhiteSpace(entity.CreatedBy) ? GetCurrentUserName(controllerContext) : entity.CreatedBy;
        entity.Modified = null;
        entity.ModifiedBy = null;
    }

    /// <summary>
    /// Copies the creation and modification audit information from one <see cref="BaseEntity"/> to another.
    /// </summary>
    /// <param name="fromEntity">The source entity to copy audit information from.</param>
    /// <param name="toEntity">The target entity to copy audit information to.</param>
    internal static void CloneCreatedInfo(BaseEntity fromEntity, BaseEntity toEntity)
    {
        toEntity.Created = fromEntity.Created;
        toEntity.CreatedBy = fromEntity.CreatedBy;
        toEntity.Modified = fromEntity.Modified;
        toEntity.ModifiedBy = fromEntity.ModifiedBy;
    }

    /// <summary>
    /// Sets the modification audit information on the specified <see cref="BaseEntity"/>.
    /// </summary>
    /// <param name="entity">The entity to set modification information for.</param>
    /// <param name="controllerContext">The controller context used to retrieve the current user.</param>
    internal static void SetModifiedInfo(BaseEntity entity, ControllerContext controllerContext)
    {
        entity.Modified ??= DateTime.UtcNow;
        entity.ModifiedBy = string.IsNullOrWhiteSpace(entity.ModifiedBy) ? GetCurrentUserName(controllerContext) : entity.ModifiedBy;
    }

    /// <summary>
    /// Copies the modification audit information from one <see cref="BaseEntity"/> to another.
    /// </summary>
    /// <param name="fromEntity">The source entity to copy modification information from.</param>
    /// <param name="toEntity">The target entity to copy modification information to.</param>
    internal static void CloneModifiedInfo(BaseEntity fromEntity, BaseEntity toEntity)
    {
        toEntity.Modified = fromEntity.Modified;
        toEntity.ModifiedBy = fromEntity.ModifiedBy;
    }

    /// <summary>
    /// Retrieves the current user's name from the specified <see cref="ControllerContext"/>.
    /// </summary>
    /// <param name="controllerContext">The controller context containing user information.</param>
    /// <returns>The current user's name, or "Unknown" if not available.</returns>
    private static string GetCurrentUserName(ControllerContext controllerContext)
    {
        return controllerContext.HttpContext.User.Identity?.Name ?? "Unknown";
    }
}
