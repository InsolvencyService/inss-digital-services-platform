using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Serialization;

namespace GovUk.Forms.Domain;

public abstract class PageModel : ContentModel
{
    public PageEditTypes EditMode { get; set; } = PageEditTypes.NotStarted;

    public string Title { get; init; } = string.Empty;
    
    public string? ReturnUrl { get; set; }

    public PageMetaData MetaData { get; init; } = new();

    public NodeId LinkedToNode { get; set; } = NodeId.Empty;

    public ContentPath PreviousPagePath { get; set; } = "/";
    
    public virtual void ClearValues()
    {
        if (this is SummaryModel)
        {
            return;
        }
            
        ReturnUrl = null;
        EditMode = PageEditTypes.NotStarted;
        PreviousPagePath = "/";
    }

    public PageModel Clone()
    {
        string json = FormSerializer.SerializePage(this);
        return FormSerializer.DeserializePage(json, GetType());
    }

    public void TransitionToEdit(ContentPath previousPagePath)
    {
        if (IsLocked())
        {
            return;
        }
        
        EditMode = PageEditTypes.Editing;
        
        if (PreviousPagePath.IsEmpty()) 
        {
            PreviousPagePath = previousPagePath;
        }
    }

    public bool IsLocked()
    {
        return (EditMode & PageEditTypes.Locked) == PageEditTypes.Locked;
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
        return !IsLocked() ? MetaData.SubmitButtonText : null;
    }
}