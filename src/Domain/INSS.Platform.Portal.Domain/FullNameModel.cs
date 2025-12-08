using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class FullNameModel : BaseModel
{
    public FullNameModel()
    {
        PathName = "full-name";
        Name = "Fullname";
    }
    
    [Required(ErrorMessage = "Enter your full name")]
    [RegularExpression("^[A-Za-z\\s\\-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens and apostrophes")]
    public string FullName { get; init; } = string.Empty;
}