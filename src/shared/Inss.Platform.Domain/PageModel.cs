using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Validation;

namespace Inss.Platform.Domain;

public sealed class PageModel
{
    public required PagePath Path { get; init; }
    
    public required string Title { get; init; }
    
    public ComponentModelList Components { get; set; } = [];

    public PagePath? PreviousPage { get; set; }
    
    public PagePathList NextPages { get; set; } = [];
    
    public Type? NextPageNavigator { get; set; }
    
    public string? SubmitButton { get; init; }
    
    public PageValidationInfo? PageValidationInfo { get; set; }
    
    public bool DisplayFullWidth { get; init; }
    
    public Type? PageValidator { get; set; }
}