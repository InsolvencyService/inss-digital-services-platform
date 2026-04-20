namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyAnnotationAttribute : Attribute
{
    public PropertyAnnotationAttribute(string category, string propertyName)
    {
        Category = category;
        PropertyName = propertyName;
    }
    
    public string Category { get; }

    public string PropertyName { get; }
}