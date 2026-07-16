namespace GovUk.Forms.Domain.Search.Formatting;

public sealed class DefaultFieldValueFormatter : FieldValueFormatter
{
    public override string Format(string?[] values)
    {
        return values.Length == 0 ? string.Empty : string.Join(' ', values).Trim();
    }
}