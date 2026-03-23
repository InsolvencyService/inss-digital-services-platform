using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowNode
{
    public NodeId Id { get; init; }
    
    public required ContentPath PagePath { get; init; }

    public NodeId[] NextNodes { get; init; } = [];
}