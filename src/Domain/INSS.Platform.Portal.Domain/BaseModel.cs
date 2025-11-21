namespace INSS.Platform.Portal.Domain;

public abstract class BaseModel
{
    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = "Index";
}