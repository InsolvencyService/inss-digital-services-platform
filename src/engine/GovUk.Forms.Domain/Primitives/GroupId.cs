namespace GovUk.Forms.Domain.Primitives;

public sealed record GroupId(string Value)
{
    public static readonly GroupId Empty = new(string.Empty);
    
    public static implicit operator string(GroupId id) => id.Value;
    
    public static implicit operator GroupId(string value) => new(value);
    
    public override string ToString() => Value;
}