namespace INSS.Platform.Portal.Domain;

public abstract class BaseModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("D");

    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = "Index";
}