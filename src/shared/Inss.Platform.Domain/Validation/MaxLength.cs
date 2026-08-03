namespace Inss.Platform.Domain.Validation;

public sealed class MaxLength : ValidationBase
{
    public required int Value { get; init; }
}