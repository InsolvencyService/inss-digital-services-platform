using GovUk.Forms.Domain;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class LoadContext
{
    public FlowNode[] Nodes { get; init; } = [];
    
    public FlowNode CurrentNode { get; init; }
    
    public FormModel Form { get; init; }
    
    public SectionModel Section { get; init; }
    
    public PageModel Page { get; init; }
    
    public string? State { get; init; }
}