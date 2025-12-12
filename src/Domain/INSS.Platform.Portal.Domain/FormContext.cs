namespace INSS.Platform.Portal.Domain;

public sealed class FormContext
{
    public string? PreviousPageUrl { get; set; }
    public string CurrentPageId { get; set; }
}