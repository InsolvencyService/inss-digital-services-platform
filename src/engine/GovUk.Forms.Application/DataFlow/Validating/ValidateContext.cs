using GovUk.Forms.Domain;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GovUk.Forms.Application.DataFlow.Validating;

public sealed class ValidateContext
{
    public FlowNode[] Nodes { get; init; } = [];
    
    public FlowNode CurrentNode { get; init; }
    
    public PageModel Page { get; init; }
}