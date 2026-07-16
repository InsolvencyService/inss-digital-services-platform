namespace GovUk.Forms.Domain.Search.Formatting;

public abstract class FieldValueFormatter
{
    public abstract string Format(string?[] values);
}