using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Infrastructure.Tests.Helpers;

/// <summary>
/// Represents a test entity used for unit testing, inheriting from <see cref="AuditableEntity"/>.
/// </summary>
public class TestEntity : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the test entity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the test entity.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
