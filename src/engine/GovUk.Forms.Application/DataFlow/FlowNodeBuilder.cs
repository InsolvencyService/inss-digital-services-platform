using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.DataFlow.Validating;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowNodeBuilder
{
    private readonly FlowchartBuilder _flowchartBuilder;
    private readonly FlowNode _node;
    private readonly IServiceCollection _services;

    internal FlowNodeBuilder(FlowchartBuilder flowchartBuilder, FlowNode node, IServiceCollection services)
    {
        _flowchartBuilder = flowchartBuilder;
        _node = node;
        _services = services;
    }

    public FlowNodeBuilder WithLoader<TLoader>() where TLoader : class, IFlowNodeLoader
    {
        _services.AddKeyedTransient<IFlowNodeLoader, TLoader>(_node.Id);
        return this;
    }
    
    public FlowNodeBuilder WithValidator<TValidator>() where TValidator : class, IFlowNodeValidator
    {
        _services.AddKeyedTransient<IFlowNodeValidator, TValidator>(_node.Id);
        return this;
    }
    
    public FlowNodeBuilder WithExecutor<TExecutor>() where TExecutor : class, IFlowNodeExecutor
    {
        _services.AddKeyedTransient<IFlowNodeExecutor, TExecutor>(_node.Id);
        return this;
    }
    
    public FlowchartBuilder Next()
    {
        return _flowchartBuilder;
    }
}