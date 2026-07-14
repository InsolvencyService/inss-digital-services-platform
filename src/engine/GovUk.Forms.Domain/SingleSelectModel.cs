namespace GovUk.Forms.Domain;

public sealed class SingleSelectModel : PageModel
{
    public string SelectedItemId { get; init; } = string.Empty;
    
    public SelectItem? SelectedItem { get; set; }
    
    public SelectItem[] Items { get; set; } = [];

    public string SelectedItemText => SelectedItem is not null ? SelectedItem.Label : string.Empty;
}