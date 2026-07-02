// ReSharper disable UnusedMember.Global - Used by binder
namespace GovUk.Forms.Domain.Primitives;

public sealed record ContentPath
{
    private const string ForwardSlash = "/";

    public ContentPath()
    {
        Value = "";
    }
    
    public ContentPath(string value)
    {
        Value = value;
    }

    public string Value
    {
        get;
        init
        {
            field = value;
            
            if (field.Length > 1 && field.EndsWith(ForwardSlash, StringComparison.InvariantCultureIgnoreCase))
            {
                field = value.TrimEnd(ForwardSlash).ToString();
            }
        }
    }

    public static implicit operator string(ContentPath path) => path.Value;
    
    public static implicit operator ContentPath(string value) => new(value);

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Value) && 
        Value.StartsWith(ForwardSlash, StringComparison.InvariantCultureIgnoreCase) &&
        !Value.EndsWith(ForwardSlash, StringComparison.InvariantCultureIgnoreCase);

    public ContentPath GetRoot()
    {
        string[] segments = Value.Split(ForwardSlash, StringSplitOptions.RemoveEmptyEntries);
        return $"{ForwardSlash}{segments.First()}";
    }

    public ContentPath? FindSection()
    {
        // The path is in one of the following forms:
        // /form
        // /form/section
        // /form/section/page
        string[] segments = Value.Split(ForwardSlash, StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 1 ? (ContentPath)$"/{segments[0]}/{segments[1]}" : null;
    }
    
    public override string ToString() => Value;
}