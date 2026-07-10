// ReSharper disable UnusedAutoPropertyAccessor.Global

using GovUk.Forms.Domain.Search.Formatting;

namespace GovUk.Forms.Domain.Search;

public sealed class SearchDefinitionField
{
    private static readonly DefaultFieldValueFormatter _defaultFieldValueFormatter = new();
    
    public required string Name { get; init; }
    
    public string? Css { get; init; }

    public  int Order { get; init; }

    public string Header { get; init; }
    
    public string? Category { get; init; }

    public bool ResultView => Category is null;

    public string? FormatterType { get; init; }

    public string GetFormattedValue(string? value)
    {
        FieldValueFormatter formatter = CreateFormatter(FormatterType);
        return formatter.Format(value);
    }

    private static FieldValueFormatter CreateFormatter(string? formatterType)
    {
        if (string.IsNullOrWhiteSpace(formatterType))
        {
            return _defaultFieldValueFormatter;
        }

        try
        {
            return (FieldValueFormatter?)Activator.CreateInstance(Type.GetType(formatterType)!, []) ?? _defaultFieldValueFormatter;
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
            return _defaultFieldValueFormatter;
        }
    }
}