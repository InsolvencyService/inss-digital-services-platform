using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
// ReSharper disable UnusedAutoPropertyAccessor.Global - might be used pending back button review

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowNodeContext
{
    public FlowNode[] Nodes { get; init; } = [];
    
    public FlowNode CurrentNode { get; init; }
    
    public FormModel Form { get; init; }
    
    public SectionModel Section { get; init; }
    
    public PageModel CurrentPage { get; init; }
    
    public PageModel? PageBeforeChanges { get; init; }
    
    public string? State { get; init; }

    public ContentPath? RefererPath { get; init; }
}