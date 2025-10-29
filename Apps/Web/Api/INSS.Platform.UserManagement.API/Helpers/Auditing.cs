using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Helpers
{
    /// <summary>
    /// Provides helper methods for setting audit information on <see cref="AuditableEntity"/> instances.
    /// </summary>
    internal static class Auditing
    {
        /// <summary>
        /// Sets the creation audit information on the specified <see cref="AuditableEntity"/>.
        /// </summary>
        /// <param name="entity">The entity to set creation information for.</param>
        /// <param name="controllerContext">The controller context used to retrieve the current user.</param>
        internal static void SetCreatedInfo(AuditableEntity entity, ControllerContext controllerContext)
        {
            entity.Created ??= DateTime.UtcNow;
            entity.CreatedBy = string.IsNullOrWhiteSpace(entity.CreatedBy) ? GetCurrentUserName(controllerContext) : entity.CreatedBy;
            entity.Modified = null;
            entity.ModifiedBy = null;
        }

        /// <summary>
        /// Sets the modification audit information on the specified <see cref="AuditableEntity"/>.
        /// </summary>
        /// <param name="entity">The entity to set modification information for.</param>
        /// <param name="controllerContext">The controller context used to retrieve the current user.</param>
        internal static void SetModifiedInfo(AuditableEntity entity, ControllerContext controllerContext)
        {
            entity.Modified ??= DateTime.UtcNow;
            entity.ModifiedBy = string.IsNullOrWhiteSpace(entity.ModifiedBy) ? GetCurrentUserName(controllerContext) : entity.ModifiedBy;
        }

        /// <summary>
        /// Retrieves the current user's name from the specified <see cref="ControllerContext"/>.
        /// </summary>
        /// <param name="controllerContext">The controller context containing user information.</param>
        /// <returns>The current user's name, or "Unknown" if not available.</returns>
        private static string GetCurrentUserName(ControllerContext controllerContext)
        {
            string? currentUser = controllerContext.HttpContext.User.Identity?.Name;
            return currentUser ?? "Unknown";
        }
    }
}
