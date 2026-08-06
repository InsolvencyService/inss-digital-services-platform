using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain;

public sealed class AppModel
{
    public required SessionId Session { get; init; }
    
    public Email? Email { get; set; }

    public PageModelList Pages { get; init; } = [];
}