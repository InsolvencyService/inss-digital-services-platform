using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace INSS.Platform.Portal.Domain;

public abstract class PageModel : BaseModel
{
    public string Title { get; set; } = string.Empty;
    
    public string PageUrl { get; set; } = string.Empty;
    
    public string NextPageUrl { get; set; } = string.Empty;
    
    public string PreviousPageUrl { get; set; } = string.Empty;
    
    public string PathName { get; init; } = "page";

    public string[] GetValues()
    {
        const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance;
        
        // Ensure UK culture for formatting
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");

        List<string> displayValueList = new();

        string[] propertiesToIgnore = PropertiesToIgnore();

        foreach (PropertyInfo property in GetType().GetProperties(propertyFlags))
        {
            if (propertiesToIgnore.Contains(property.Name))
            {
                continue;
            }

            object? value = property.GetValue(this, null);

            if (value is null)
            {
                continue;
            }

            DisplayFormatAttribute? displayValueFormat = property.GetCustomAttribute<DisplayFormatAttribute>();

            string? displayValue = displayValueFormat?.DataFormatString is not null
                ? string.Format(CultureInfo.GetCultureInfo("en-GB"), displayValueFormat.DataFormatString, value)
                : value.ToString();

            if (!string.IsNullOrWhiteSpace(displayValue))
            {
                displayValueList.Add(displayValue);
            }
        }

        return displayValueList.ToArray();
    }

    protected virtual string[] PropertiesToIgnore()
    {
        return [
            nameof(Title),
            nameof(PageUrl),
            nameof(NextPageUrl),
            nameof(PreviousPageUrl),
            nameof(PathName),
            nameof(Controller),
            nameof(Action)
        ];
    }
}