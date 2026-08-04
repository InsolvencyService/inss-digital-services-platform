using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Domain;

public sealed class PageModel
{
    public required PagePath Path { get; init; }
    
    public required string Title { get; init; }
    
    public ComponentList Components { get; set; } = [];

    public PagePath? PreviousPage { get; set; }
    
    public PagePathList NextPages { get; init; } = [];
    
    public Type? NextPageNavigator { get; init; }
    
    public Content SubmitButton { get; init; }
    
    public PageValidationInfo? PageValidationInfo { get; set; }
}