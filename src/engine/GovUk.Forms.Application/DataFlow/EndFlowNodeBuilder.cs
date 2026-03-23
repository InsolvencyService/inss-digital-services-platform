using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.DataFlow;

public sealed class EndFlowNodeBuilder
{
    private readonly SectionModel _section;
    private readonly FlowNode _node;
    private readonly IServiceCollection _services;
    private readonly List<FlowNode> _nodes;

    internal EndFlowNodeBuilder(SectionModel section, List<FlowNode> nodes, FlowNode node, IServiceCollection services)
    {
        _section = section;
        _nodes = nodes;
        _node = node;
        _services = services;
    }

    public EndFlowNodeBuilder WithLoader<TLoader>() where TLoader : class, IFlowNodeLoader
    {
        _services.AddKeyedSingleton<IFlowNodeLoader, TLoader>(_node.Id);
        return this;
    }
    
    public EndFlowNodeBuilder WithExecutor<TExecutor>() where TExecutor : class, IFlowNodeExecutor
    {
        _services.AddKeyedSingleton<IFlowNodeExecutor, TExecutor>(_node.Id);
        return this;
    }
    
    public void BuildAndRegister()
    {
        IServiceProvider serviceProvider = _services.BuildServiceProvider();
        ILogger<Flowchart> logger = serviceProvider.GetRequiredService<ILogger<Flowchart>>();
        Flowchart flowchart = new(serviceProvider, logger);

        foreach (FlowNode node in _nodes)
        {
            flowchart.AddNode(node);
        }
        
        FlowchartValidator flowchartValidator = new(_nodes.ToArray());
        flowchartValidator.Validate(_section, _services.BuildServiceProvider());
        
        _services.AddKeyedSingleton<IFlowchart>(_section.Path, flowchart);
    }
}