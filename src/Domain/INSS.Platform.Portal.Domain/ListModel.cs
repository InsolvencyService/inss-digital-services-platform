using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a model for managing a list of items with confirmation and selection functionality.
/// </summary>
public class ListModel
{
    /// <summary>
    /// Gets or sets the collection of item names.
    /// </summary>
    public List<string> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the index of the selected item in the list.
    /// </summary>
    public int ItemIndex { get; set; }

    /// <summary>
    /// Gets or sets the confirmation value.
    /// </summary>
    /// <remarks>
    /// This field is required. An error message will be shown if not selected.
    /// </remarks>
    [Required(ErrorMessage = "Select an option")]
    public Confirmation? Confirm { get; set; }

    /// <summary>
    /// Gets or sets the name of the selected item.
    /// </summary>
    public string? ItemName { get; set; }
}
