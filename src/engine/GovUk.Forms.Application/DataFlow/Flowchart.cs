using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.DataFlow;

public sealed class Flowchart : IFlowchart
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Flowchart> _logger;
    private readonly Dictionary<NodeId, FlowNode> _nodes = new();

    public Flowchart(IServiceProvider serviceProvider, ILogger<Flowchart> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public FlowNode[] Nodes => _nodes.Select(n => n.Value).ToArray();
    
    public void AddNode(FlowNode node)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(node.Id);

        if (!_nodes.TryAdd(node.Id, node))
        {
            throw new FlowchartException($"The node Id {node.Id} already exists.");
        }
    }
    
    public async ValueTask<ContentPath> PreProcessAsync(
        FormModel form, 
        SectionModel section, 
        PageModel page, 
        Dictionary<string, string?> queryParams)
    {
        _logger.LoadingPage(page.Path, section.Title);
        
        FlowNode node = GetNode(page.LinkedToNode);
        NodeId? nextNodeId = await LoadAndGetOptionalAltNodeAsync(node, form, section, page, queryParams);
        PageModel pageAssociatedToNode = page;

        // TODO: We might want to loop here but assume if not null we do one more level
        if (nextNodeId is not null)
        {
            ContentPath nextPagePath = GetPagePath(nextNodeId);
            pageAssociatedToNode = section.Pages.GetPage(nextPagePath);
            pageAssociatedToNode.LinkedToNode = nextNodeId;
        }

        section.SetInProgress();
        
        await UpdateBackButtonAsync(form, section, page);
        return pageAssociatedToNode.Path;
    }
    
    public async ValueTask<ValidationResult[]> ValidateAsync(FormModel form, SectionModel section, PageModel page)
    {
        _logger.ValidatingPage(page.Path);
        
        FlowNode node = GetNode(page.LinkedToNode);
        IFlowNodeValidator validator = _serviceProvider.GetKeyedService<IFlowNodeValidator>(node.Id) ?? DefaultFlowNodeValidator.Default;
        FlowNodeContext context = new()
        {
            Nodes = Nodes,
            CurrentNode = node,
            Form = form,
            Section = section,
            CurrentPage = page
        };
        return await validator.ValidateAsync(context);
    }
    
    public async ValueTask<ContentPath> ProcessAsync(FormModel form, SectionModel section, PageModel page)
    {
        _logger.ProcessingPage(page.Path, section.Title);
        
        FlowNode node = GetNode(page.LinkedToNode);
        
        PageModel targetPage = section.Pages.GetPage(page.Path);
        PageModel pageBeforeChanges = targetPage.Clone();
        CopyPageData(page, targetPage);
        
        NodeId? nextNodeId = await GetNextNodeForUpdatedPageAsync(node, page, pageBeforeChanges, form, section);

        // If this is the first visit then we just set the link to the next node, otherwise we need to determine if the data entered
        // has changed the route to go down. If it has then we reset downstream page. If not then we can return to the previous page
        // e.g. return url if set or continue setting the next page up
        if (targetPage.LinkedToNextNode is null)
        {
            targetPage.LinkedToNextNode = nextNodeId;
        }
        else
        {
            bool resetPages = targetPage.LinkedToNextNode != nextNodeId;

            targetPage.LinkedToNextNode = nextNodeId;
            
            if (!resetPages && section.ReturnUrl is not null)
            {
                return section.ReturnUrl;
            }
        }
        
        ContentPath nextPagePath = form.Path;
        
        if (nextNodeId is not null)
        {
            nextPagePath = GetPagePath(nextNodeId);

            PageModel nextPage = section.Pages.GetPage(nextPagePath);
            nextPage.LinkedToNode = nextNodeId;
        }

        return nextPagePath;
    }
    
    public void TransitionPageToStart(PageModel page)
    {
        foreach (var node in _nodes.Where(node => node.Value.PagePath == page.Path))
        {
            page.LinkedToNode = node.Value.Id;
            return;
        }

        throw new FlowchartException($"Unable to find a node Id for page with path {page.Path}");
    }

    private async ValueTask UpdateBackButtonAsync(FormModel form, SectionModel section, PageModel page)
    {
        FlowNode node = GetNode(page.LinkedToNode);
        IFlowNodePreviousPathProvider flowNodePreviousPathProvider =
            _serviceProvider.GetKeyedService<IFlowNodePreviousPathProvider>(section.Path)
            ?? _serviceProvider.GetRequiredService<IFlowNodePreviousPathProvider>();
        FlowNodeContext context = new() { Nodes = Nodes, CurrentNode = node, Form = form, Section = section, CurrentPage = page };
        await flowNodePreviousPathProvider.UpdateAsync(context);
    }
    
    private FlowNode GetNode(NodeId? nodeId)
    {
        if (nodeId is null || !_nodes.TryGetValue(nodeId, out FlowNode? node))
        {
            throw new FlowchartException($"Start node '{nodeId}' not found.");
        }

        return node;
    }
    
    private ContentPath GetPagePath(NodeId nodeId)
    {
        return !_nodes.TryGetValue(nodeId, out FlowNode? node) 
            ? throw new FlowchartException($"Node ID '{nodeId}' not found.")
            : node.PagePath;
    }

    private async ValueTask<NodeId?> LoadAndGetOptionalAltNodeAsync(
        FlowNode node, 
        FormModel form, 
        SectionModel section, 
        PageModel page, 
        Dictionary<string, string?> queryParams)
    {
        IFlowNodeLoader loader = _serviceProvider.GetKeyedService<IFlowNodeLoader>(node.Id) ?? NoopFlowNodeLoader.Default;
        FlowNodeContext context = new()
        {
            Nodes = Nodes, CurrentNode = node, Form = form, Section = section, CurrentPage = page, QueryParams = queryParams
        };
        return await loader.LoadAsync(context);
    }
    
    private async ValueTask<NodeId?> GetNextNodeForUpdatedPageAsync(
        FlowNode node, 
        PageModel updatedPage, 
        PageModel pageBeforeChanges,
        FormModel form, 
        SectionModel section)
    {
        IFlowNodeExecutor executor = _serviceProvider.GetKeyedService<IFlowNodeExecutor>(node.Id) ?? NoopFlowNodeExecutor.Default;
        FlowNodeContext context = new()
        {
            Nodes = Nodes, 
            CurrentNode = node, 
            Form = form, 
            Section = section, 
            CurrentPage = updatedPage, 
            PageBeforeChanges = pageBeforeChanges
        };
        return await executor.ExecuteAsync(context);
    }
    
    private static void CopyPageData(PageModel sourcePage, PageModel targetPage)
    {
        sourcePage.CopyTo(targetPage);
        targetPage.SetCompleted();
    }
}