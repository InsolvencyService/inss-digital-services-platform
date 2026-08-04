using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain;

public sealed class App
{
    public required SessionId Session { get; init; }
    
    public Email? Email { get; init; }

    public PageModelList Pages { get; init; } = [];
}