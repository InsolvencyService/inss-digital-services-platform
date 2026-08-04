namespace Inss.Platform.Domain.Validation;

public sealed class PageValidationInfo
{
    public required PageValidationError[] Errors { get; init; }
}