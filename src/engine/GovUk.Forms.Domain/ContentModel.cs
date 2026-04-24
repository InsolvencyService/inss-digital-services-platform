using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Exceptions;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public abstract class ContentModel
{
    protected ContentModel()
    {
        ViewName = $"_{GetType().Name.Replace("Model", string.Empty)}";
    }
    
    public ContentId Id { get; set; } = ContentId.New();

    public ContentPath Path { get; init; } = "/";
    
    public string ViewName { get; init; }

    public string? EncodingType { get; init; }
    
    public bool FullWidthLayout { get; set; }
    
    public SubmitTypes SubmitType { get; init; } = SubmitTypes.Form;
    
    public string TypeName => GetType().FullName!;
    
    public T As<T>() where T : ContentModel
    {
        if (this is T result)
        {
            return result;
        }

        throw new ModelException($"Cannot cast to type {typeof(T)}.");
    }
}