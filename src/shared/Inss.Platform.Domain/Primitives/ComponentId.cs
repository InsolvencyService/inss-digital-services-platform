namespace Inss.Platform.Domain.Primitives;

public sealed record ComponentId(string Value)
{
    public static implicit operator string(ComponentId id) => id.Value;
    
    public static implicit operator ComponentId(string value) => new(value);
    
    public override string ToString() => Value;
}