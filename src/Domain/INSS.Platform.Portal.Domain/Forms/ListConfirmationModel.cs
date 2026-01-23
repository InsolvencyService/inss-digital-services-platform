using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

// <summary>
// Represents a confirmation model for a list item, including the question, confirmation response, and item index.
// </summary>
public class ListConfirmationModel
{
    // <summary>
    // Gets or sets the question to be displayed for confirmation.
    // </summary>
    public string? Question { get; set; }

    // <summary>
    // Gets or sets the confirmation response.
    // </summary>
    [Required(ErrorMessage = "Select an option")]
    public Confirmation? Confirm { get; set; }

    // <summary>
    // Gets or sets the index of the item in the list.
    // </summary>
    public int ItemIndex { get; set; }
}
