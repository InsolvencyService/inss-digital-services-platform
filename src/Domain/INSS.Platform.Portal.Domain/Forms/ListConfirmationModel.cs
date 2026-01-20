using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

public class ListConfirmationModel
{
    public string? Question { get; set; }

    [Required(ErrorMessage = "Select an option")]
    public Confirmation? Confirm { get; set; }

    public int ItemIndex { get; set; }
}
