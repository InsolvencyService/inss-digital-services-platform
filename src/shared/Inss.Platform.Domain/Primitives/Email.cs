namespace Inss.Platform.Domain.Primitives;

public sealed record Email(string Value)
{
    public static implicit operator string(Email email) => email.Value;
    
    public static implicit operator Email(string value) => new(value);
    
    public override string ToString() => Value;
}