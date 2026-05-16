using GovUk.Forms.Domain;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class ExecuteContext
{
    public FlowNode[] Nodes { get; init; } = [];

    public FlowNode CurrentNode { get; init; }

    public FormModel Form { get; init; }

    public SectionModel Section { get; init; }

    public PageModel UpdatedPage { get; init; }
}