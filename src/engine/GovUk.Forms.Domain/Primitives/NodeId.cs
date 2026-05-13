namespace GovUk.Forms.Domain.Primitives;

public sealed record NodeId(string Value)
{
    public static readonly NodeId Empty = new(string.Empty);
    
    public static implicit operator string(NodeId id) => id.Value;
    
    public static implicit operator NodeId(string value) => new(value);
    
    public override string ToString() => Value;
}