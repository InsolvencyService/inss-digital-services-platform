using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public sealed class PageMetaData
{
    public string? Question { get; set; }
    
    public string? Hint { get; set; }
    
    public string? Description { get; set; }

    public GroupId Group { get; set; } = GroupId.Empty;

    public string? SubmitButtonText { get; set; }
    
    public void CopyTo(PageMetaData pageMetaData)
    {
        pageMetaData.Question = Question;
        pageMetaData.Hint = Hint;
        pageMetaData.Description = Description;
        pageMetaData.Group = Group;
        pageMetaData.SubmitButtonText = SubmitButtonText;
    }
}