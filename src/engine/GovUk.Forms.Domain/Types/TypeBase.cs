namespace GovUk.Forms.Domain.Types;

public abstract class TypeBase
{
    public string ViewName => $"Types/_{GetType().Name}";
}