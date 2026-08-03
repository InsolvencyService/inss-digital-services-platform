namespace Inss.Platform.Domain.Primitives;

public sealed record SessionId(string Value)
{
    public static implicit operator string(SessionId id) => id.Value;
    
    public static implicit operator SessionId(string value) => new(value);
    
    public override string ToString() => Value;
}