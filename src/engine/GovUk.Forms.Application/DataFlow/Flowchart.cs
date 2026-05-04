using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
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

    private FlowNode[] Nodes => _nodes.Select(n => n.Value).ToArray();
    
    public void AddNode(FlowNode node)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(node.Id);

        if (!_nodes.TryAdd(node.Id, node))
        {
            throw new FlowchartException($"The node Id {node.Id} already exists.");
        }
    }
    
    public async ValueTask<ContentPath> PreProcessAsync(FormModel form, SectionModel section, PageModel page, string? state)
    {
        _logger.LoadingPage(page.Path, section.Title);
        
        FlowNode node = GetNode(page.LinkedToNode);
        NodeId? nextNodeId = await LoadAndGetOptionalAltNodeAsync(node, form, section, page, state);
        PageModel pageAssociatedToNode = page;

        // TODO: We might want to loop here but assume if not null we do one more level
        if (nextNodeId is not null)
        {
            ContentPath nextPagePath = GetPagePath(nextNodeId);
            pageAssociatedToNode = section.Pages.GetPage(nextPagePath);
            pageAssociatedToNode.LinkedToNode = nextNodeId;
        }

        ContentPath previousPagePath = form.Sections.Count > 1 ? form.Path : "/";
        pageAssociatedToNode.TransitionToEdit(previousPagePath);
        section.SetInProgress();

        return await ValueTask.FromResult(pageAssociatedToNode.Path);
    }
    
    public async ValueTask<ValidationResult[]> ValidateAsync(PageModel page)
    {
        _logger.ValidatingPage(page.Path);
        
        FlowNode node = GetNode(page.LinkedToNode);
        IFlowNodeValidator validator = _serviceProvider.GetKeyedService<IFlowNodeValidator>(node.Id) ?? DefaultFlowNodeValidator.Default;
        ValidateContext context = new() { Nodes = Nodes, CurrentNode = node, Page = page };
        return await validator.ValidateAsync(context);
    }
    
    public async ValueTask<ContentPath> ProcessAsync(FormModel form, SectionModel section, PageModel page)
    {
        _logger.ProcessingPage(page.Path, section.Title);
        
        FlowNode node = GetNode(page.LinkedToNode);
        PageModel targetPage = section.Pages.GetPage(page.Path);
        
        NodeId? currentPageNodeId = await GetNextNodeForSavedPageAsync(node, targetPage, form, section);

        CopyPageData(page, targetPage);
        
        NodeId? nextNodeId = await GetNextNodeForUpdatedPageAsync(node, page, form, section);

        ContentPath nextPagePath = GetNextPagePath(targetPage, currentPageNodeId, nextNodeId, form, section);

        PageModel? nextPage = section.Pages.FindPage(nextPagePath);
        nextPage?.PreviousPagePath = page.Path;
        
        return await ValueTask.FromResult(nextPagePath);
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
        string? state)
    {
        IFlowNodeLoader loader = _serviceProvider.GetKeyedService<IFlowNodeLoader>(node.Id) ?? NoopFlowNodeLoader.Default;
        LoadContext context = new() { Nodes = Nodes, CurrentNode = node, Form = form, Section = section, Page = page, State = state };
        return await loader.LoadAsync(context);
    }
    
    private async ValueTask<NodeId?> GetNextNodeForSavedPageAsync(
        FlowNode node, 
        PageModel targetPage, 
        FormModel form, 
        SectionModel section)
    {
        IFlowNodeExecutor executor = _serviceProvider.GetKeyedService<IFlowNodeExecutor>(node.Id) ?? NoopFlowNodeExecutor.Default;
        PageModel copyOfCurrentPage = targetPage.Clone();
        ExecuteContext context = new() 
        { 
            Nodes = Nodes, CurrentNode = node, Form = form, Section = section, UpdatedPage = copyOfCurrentPage
        };
        return await executor.ExecuteAsync(context);
    }

    private async ValueTask<NodeId?> GetNextNodeForUpdatedPageAsync(
        FlowNode node, 
        PageModel updatedPage, 
        FormModel form, 
        SectionModel section)
    {
        IFlowNodeExecutor executor = _serviceProvider.GetKeyedService<IFlowNodeExecutor>(node.Id) ?? NoopFlowNodeExecutor.Default;
        ExecuteContext context = new()
        {
            Nodes = Nodes, CurrentNode = node, Form = form, Section = section, UpdatedPage = updatedPage, FinalExecuteStep = true
        };
        return await executor.ExecuteAsync(context);
    }

    private ContentPath GetNextPagePath(
        PageModel targetPage, 
        NodeId? currentPageNodeId, 
        NodeId? nextNodeId, 
        FormModel form, 
        SectionModel section)
    {
        if (currentPageNodeId != nextNodeId)
        {
            // TODO: Bug - we need to use the flowchart nodes to decide what to reset in case the page order is not correct
            section.Pages.ResetDownstream(targetPage);
        }
        else
        {
            if (targetPage.ReturnUrl is not null)
            {
                return targetPage.ReturnUrl;
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
    
    private static void CopyPageData(PageModel sourcePage, PageModel targetPage)
    {
        sourcePage.CopyTo(targetPage);
        targetPage.SetCompleted();
    }
}