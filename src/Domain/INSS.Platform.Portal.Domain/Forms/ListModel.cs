using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

public class ListModel
{
    public List<string> Items { get; set; } = new();

    public int ItemIndex { get; set; }

    [Required(ErrorMessage = "Select an option")]
    public Confirmation? Confirm { get; set; }

    public string? ItemName { get; set; }
}
