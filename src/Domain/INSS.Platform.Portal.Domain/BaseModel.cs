using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace INSS.Platform.Portal.Domain;

public abstract class BaseModel
{
    private const BindingFlags DerivedTypeBinding =
        BindingFlags.Instance |
        BindingFlags.Public |
        BindingFlags.DeclaredOnly |
        BindingFlags.GetProperty |
        BindingFlags.SetProperty;
    
    protected BaseModel()
    {
        Id = Guid.NewGuid().ToString("D");
        Kind = GetType().Name;
        ViewName = $"_{GetType().Name.Replace("Model", "")}";
        PathName = "page";
        PreviousPageUrl = "/tasks";
    }
    
    public string Id { get; set; }

    public string Kind { get; init; }

    public string Name { get; init; }
    
    public string PathName { get; init; }
    
    public string ViewName { get; init; }
    
    public string PageUrl { get; set; }
    
    public string PreviousPageUrl { get; set; }
    
    public void CopyTo(BaseModel pageModel)
    {
        PropertyInfo[] properties = GetType().GetProperties(DerivedTypeBinding);

        foreach (PropertyInfo property in properties)
        {
            if (property.CanWrite)
            {
                property.SetValue(pageModel, property.GetValue(this));
            }
        }
    }
    
    public BaseModel Clone()
    {
        BaseModel clone = (BaseModel)Activator.CreateInstance(GetType())!;
        clone.Id = Id;

        PropertyInfo[] properties = GetType().GetProperties(DerivedTypeBinding);

        foreach (PropertyInfo property in properties)
        {
            property.SetValue(clone, property.GetValue(this));
        }
        
        return clone;
    }
    
    public void Reset()
    {
        Id = Guid.NewGuid().ToString("D");

        PropertyInfo[] properties = GetType().GetProperties(DerivedTypeBinding);

        foreach (PropertyInfo property in properties)
        {
            property.SetValue(this, null);
        }
    }
    
    public string[] GetValues()
    {
        List<string> displayValueList = [];

        PropertyInfo[] props = GetType().GetProperties(DerivedTypeBinding);
        
        foreach (PropertyInfo property in props)
        {
            object? value = property.GetValue(this, null);

            if (value is null)
            {
                continue;
            }

            if (typeof(IEnumerable<BaseModel>).IsAssignableFrom(property.PropertyType))
            {
                foreach (BaseModel page in (IEnumerable<BaseModel>)value)
                {
                    displayValueList.AddRange(page.GetValues());
                }

                continue;
            }

            DisplayFormatAttribute? displayValueFormat = property.GetCustomAttribute<DisplayFormatAttribute>();

            string? displayValue = displayValueFormat?.DataFormatString is not null
                ? string.Format(CultureInfo.CurrentCulture, displayValueFormat.DataFormatString, value)
                : value.ToString();

            if (!string.IsNullOrWhiteSpace(displayValue))
            {
                displayValueList.Add(displayValue);
            }
        }

        return [.. displayValueList];
    }
}