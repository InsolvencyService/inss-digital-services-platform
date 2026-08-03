namespace Inss.Platform.Domain.Primitives;

public sealed record PagePath(string Value)
{
    public static implicit operator string(PagePath path) => path.Value;
    
    public static implicit operator PagePath(string value) => new(value);
    
    public override string ToString() => Value;
}