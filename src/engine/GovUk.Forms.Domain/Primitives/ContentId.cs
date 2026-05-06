namespace GovUk.Forms.Domain.Primitives;

public sealed record ContentId(string Value)
{
    public static readonly ContentId Empty = new(string.Empty);
    
    public static implicit operator string(ContentId id) => id.Value;
    
    public static implicit operator ContentId(string value) => new(value);

    public static ContentId New() => new(Guid.NewGuid().ToString());
    
    public override string ToString() => Value;
}