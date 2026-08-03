using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain;

public sealed class Page
{
    public required PagePath Path { get; init; }
    
    public required string Title { get; init; }
    
    public ComponentList Components { get; init; } = [];

    public PagePath? PreviousPage { get; init; }
    
    public PagePathList NextPages { get; init; } = [];
    
    public Type? NextPageNavigator { get; init; }
}