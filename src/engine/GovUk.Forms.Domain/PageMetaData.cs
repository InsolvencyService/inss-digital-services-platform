using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public sealed class PageMetaData
{
    public string? Question { get; set; }
    
    public string? Hint { get; set; }
    
    public string? Description { get; set; }

    public GroupId Group { get; set; } = GroupId.Empty;

    public string SubmitButtonText { get; set; } = "Save and continue";
}