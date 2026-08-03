namespace Inss.Platform.Domain.Primitives;

public sealed record Content(string Value)
{
    public override string ToString() => Value;
}