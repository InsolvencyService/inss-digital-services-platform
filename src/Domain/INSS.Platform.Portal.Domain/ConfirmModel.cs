using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public class ConfirmModel : BaseModel
{
    public ConfirmModel()
    {
        PathName = "confirm";
        Name = "Confirm";
    }

    [Required(ErrorMessage = "Choose Yes or No to confirm")]
    public bool Confirmed { get; init; }
}