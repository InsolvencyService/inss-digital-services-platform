namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public abstract class Error
{
    public string Key { get; init; }
    
    public string GetCategory()
    {
        ValidationInfo validationInfo = ValidationInfoLookup.Get(Key);
        return validationInfo.Category;
    }
}