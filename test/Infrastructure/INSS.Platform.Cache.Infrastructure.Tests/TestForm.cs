using INSS.Platform.Portal.Domain.Abstract;

namespace INSS.Platform.Cache.Infrastructure.Tests;

/// <summary>
/// Represents a test implementation of <see cref="FormBase"/> for unit testing purposes.
/// </summary>
public class TestForm : FormBase
{
    /// <summary>
    /// Gets or sets the name property on the test form.
    /// </summary>
    public string? Name { get; set; }
}
