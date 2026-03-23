namespace GovUk.Forms.Domain.Primitives;

public sealed record ContentPath
{
    private const string ForwardSlash = "/";
    
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
            
            if (field.EndsWith(ForwardSlash, StringComparison.InvariantCultureIgnoreCase))
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

    public bool IsEmpty() => string.IsNullOrWhiteSpace(Value) || Value == ForwardSlash;

    public ContentPath GetRoot()
    {
        string[] segments = Value.Split(ForwardSlash, StringSplitOptions.RemoveEmptyEntries);
        return $"{ForwardSlash}{segments.First()}";
    }
    
    public override string ToString() => Value;
}