using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Serialization;

namespace GovUk.Forms.Domain;

public abstract class PageModel : ContentModel
{
    public string Title { get; init; } = string.Empty;
    
    public string? ReturnUrl { get; set; }

    public PageMetaData MetaData { get; init; } = new();

    public NodeId LinkedToNode { get; set; } = NodeId.Empty;

    public ContentPath? PreviousPagePath { get; set; }
    
    public DateTimeOffset? CompletedDate { get; set; }
    
    public virtual void ClearValues()
    {
        if (this is SummaryModel)
        {
            return;
        }
            
        ReturnUrl = null;
        CompletedDate = null;
        PreviousPagePath = "/";
    }

    public PageModel Clone()
    {
        string json = FormSerializer.SerializePage(this);
        return FormSerializer.DeserializePage(json, GetType());
    }

    public void SetCompleted()
    {
        CompletedDate = DateTimeOffset.Now;
    }

    public virtual string[] GetSummaryInfo()
    {
        return [];
    }

    public virtual void CopyTo(PageModel target)
    {
    }

    public virtual string? GetButtonText()
    {
        return MetaData.SubmitButtonText;
    }
}