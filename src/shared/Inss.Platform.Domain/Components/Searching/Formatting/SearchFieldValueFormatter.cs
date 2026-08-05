namespace Inss.Platform.Domain.Components.Searching.Formatting;

public abstract class SearchFieldValueFormatter
{
    private static readonly SearchFieldValueFormatter _defaultSearchFieldValueFormatter = new DefaultSearchFieldValueFormatter();
    
    public abstract string Format(string?[] values);
    
    public static SearchFieldValueFormatter CreateFormatter(string? formatterType)
    {
        if (string.IsNullOrWhiteSpace(formatterType))
        {
            return _defaultSearchFieldValueFormatter;
        }

        try
        {
            return (SearchFieldValueFormatter?)Activator.CreateInstance(Type.GetType(formatterType)!, []) ?? _defaultSearchFieldValueFormatter;
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
            return _defaultSearchFieldValueFormatter;
        }
    }
}