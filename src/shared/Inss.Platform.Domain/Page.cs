using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain;

public sealed class Page
{
    public required PagePath Path { get; init; }
    
    public required string Title { get; init; }
    
    public ComponentList Components { get; set; } = [];

    public PagePath? PreviousPage { get; init; }
    
    public PagePathList NextPages { get; init; } = [];
    
    public Type? NextPageNavigator { get; init; }
    
    public Content SubmitButton { get; init; }
    
    public PageValidationInfo? PageValidationInfo { get; set; }
}

public sealed class PageValidationInfo
{
    public required PageValidationError[] Errors { get; init; }
}

public sealed class PageValidationError
{
    public required string[] Properties { get; init; }
    
    public required string Message { get; init; }
}