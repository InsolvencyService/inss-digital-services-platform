using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public class ConfirmModel : PageModel
{
    public ConfirmModel()
    {
        PathName = "confirm";
        Title = "Confirm";
        Controller = "Confirm";
    }

    public string? ConfirmationId { get; set; }

    [Required(ErrorMessage = "Choose Yes or No to confirm")]
    public bool Confirm { get; set; }
}
