using System.Reflection;
using GovUk.Forms.Domain.Attributes;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Exceptions;
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
            
        IEnumerable<PropertyInfo> properties = GetType().GetProperties().Where(
            p => p.GetCustomAttribute<CopyableAttribute>() is not null);

        ReturnUrl = null;
        EditMode = PageEditTypes.NotStarted;
        PreviousPagePath = "/";
            
        foreach (PropertyInfo property in properties)
        {
            if (property.CanWrite)
            {
                property.SetValue(this, null);
            }
        }
    }

    public void CopyTo(PageModel target)
    {
        if (GetType() != target.GetType())
        {
            throw new ModelException(
                $"The target type to copy to {target.GetType()} does not match the page type {GetType()}.");
        }

        IEnumerable<PropertyInfo> properties = GetType().GetProperties().Where(p => p.GetCustomAttribute<CopyableAttribute>() is not null);

        foreach (PropertyInfo property in properties)
        {
            if (!property.CanWrite)
            {
                continue;
            }

            property.SetValue(target, property.GetValue(this));
        }
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
}