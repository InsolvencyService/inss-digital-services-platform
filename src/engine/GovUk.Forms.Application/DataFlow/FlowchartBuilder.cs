using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowchartBuilder
{
    private readonly SectionModel _section;
    private readonly IServiceCollection _services;
    private readonly List<FlowNode> _nodes = [];

    private FlowchartBuilder(SectionModel section, IServiceCollection services)
    {
        _section = section;
        _services = services;
    }

    public static FlowchartBuilder ForSection(SectionModel section, IServiceCollection services)
    {
        return new FlowchartBuilder(section, services);
    }

    public FlowNodeBuilder AddTransitionNode(NodeId nodeId, ContentPath pagePath, NodeId nextNodeId)
    {
        FlowNode node = new() { Id = nodeId, PagePath = pagePath, NextNodes = [nextNodeId] };
        _nodes.Add(node);
        return new FlowNodeBuilder(this, node, _services);
    }
    
    public FlowNodeBuilder AddSpurNode(NodeId nodeId, ContentPath pagePath, NodeId nextNodeId, NodeId spurNodeId)
    {
        FlowNode node = new() { Id = nodeId, PagePath = pagePath, NextNodes = [nextNodeId, spurNodeId] };
        _nodes.Add(node);
        return new FlowNodeBuilder(this, node, _services);
    }
    
    public EndFlowNodeBuilder AddEndNode(NodeId nodeId, ContentPath pagePath)
    {
        FlowNode node = new() { Id = nodeId, PagePath = pagePath };
        _nodes.Add(node);
        return new EndFlowNodeBuilder(_section, _nodes, node, _services);
    }
    
    public FlowNodeBuilder AddDecisionNode(NodeId nodeId, ContentPath pagePath, params NodeId[] nextNodeIds)
    {
        if (nextNodeIds.Length < 2)
        {
            throw new FlowchartException("You require at least 2 nodes for a decision.");
        }
        
        FlowNode node = new() { Id = nodeId, PagePath = pagePath, NextNodes = nextNodeIds };
        _nodes.Add(node);
        return new FlowNodeBuilder(this, node, _services);
    }
}