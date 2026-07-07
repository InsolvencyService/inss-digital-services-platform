using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.PageFlow;

public class TreeViewManager : ITreeViewManager
{
    private readonly ITreeNodeFactory _treeNodeFactory;
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TreeViewManager> _logger;

    public TreeViewManager(ITreeNodeFactory treeNodeFactory, IPagePropertiesProvider pagePropertiesProvider, IServiceProvider serviceProvider, ILogger<TreeViewManager> logger)
    {
        _treeNodeFactory = treeNodeFactory;
        _pagePropertiesProvider = pagePropertiesProvider;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public ContentPath TransitionToStart(SectionModel section)
    {
        TreeNode rootNode = _treeNodeFactory.GetRootNode(section.Path);
        section.TreeNodeId = rootNode.Id;
        return rootNode.PagePath;
    }
    
    public async ValueTask<PageModel> LoadAsync(FormModel form, SectionModel section, ContentPath path, Dictionary<string, string?> queryParams)
    {
        _logger.LoadingPage(path, section.Title);
        
        if (section.TreeNodeId is null)
        {
            throw new InvalidOperationException("The section tree node Id is unset."); // TODO:
        }
        
        //IPagePropertiesProvider pagePropertiesProvider = _serviceProvider.GetRequiredService<IPagePropertiesProvider>();
        TreeNode rootNode = _treeNodeFactory.GetRootNode(section.Path);
        TreeNode pageNode = rootNode.GetNode(section.TreeNodeId);

        // Handle back button navigation
        if (pageNode.PagePath != path)
        {
            pageNode = rootNode.GetNodeForPath(path);

            PageModel parentPage = CreatePage(section, pageNode);
            section.TreeNodeId = pageNode.Id;

            await RunLoaderAsync(form, section, parentPage, pageNode, queryParams);
            
            if (section.ReturnUrl is not null)
            {
                _pagePropertiesProvider.PreviousPagePath = section.ReturnUrl;
            }
            else
            {
                TreeNode? parentNode2 = rootNode.FindParent(pageNode);
                _pagePropertiesProvider.PreviousPagePath = parentNode2?.PagePath ?? "/";
            }

            return parentPage;
        }

        PageModel page = CreatePage(section, pageNode);
        
        await RunLoaderAsync(form, section, page, pageNode, queryParams);

        TreeNode? parentNode = rootNode.FindParent(pageNode);
        _pagePropertiesProvider.PreviousPagePath = parentNode?.PagePath ?? "/";
        
        return page;
    }
    
    public async ValueTask<ValidationResult[]> ValidateAsync(FormModel form, SectionModel section, PageModel page)
    {
        _logger.ValidatingPage(page.Path);
        
        TreeNode rootNode = _treeNodeFactory.GetRootNode(section.Path);
        TreeNode? node = rootNode.FindNode(section.TreeNodeId!); // TODO: Handle !

        if (node is null)
        {
            throw new InvalidOperationException($"Unable to find the tree node for {section.TreeNodeId}."); // TODO: Fix
        }
        
        IPageValidator validator = _serviceProvider.GetKeyedService<IPageValidator>(node.FlowNodeId) ?? DefaultPageValidator.Default;
        ValidatePageContext context = new() { Form = form, Section = section, CurrentPage = page };
        
        await validator.ValidateAsync(context);

        if (context.ValidationResults.Count > 0)
        {
            page.Path = node.PagePath;
            page.MetaData2 = node.MetaData;
            page.LinkedToNextNode = node.Id;
            
            IPagePropertiesProvider pagePropertiesProvider = _serviceProvider.GetRequiredService<IPagePropertiesProvider>();
            TreeNode? parentNode = rootNode.FindParent(node);
            pagePropertiesProvider.PreviousPagePath = parentNode?.PagePath ?? "/";
        }

        return context.ValidationResults.ToArray();
    }
    
    public async ValueTask<ContentPath> SaveAsync(FormModel form, SectionModel section, PageModel page)
    {
        _logger.ProcessingPage(page.Path, section.Title);
        
        TreeNode rootNode = _treeNodeFactory.GetRootNode(section.Path);
        TreeNode? node = rootNode.FindNode(section.TreeNodeId!); // TODO: Handle !
        
        if (node is null)
        {
            throw new InvalidOperationException($"Unable to find the tree node for {section.TreeNodeId}."); // TODO: Fix
        }
        
        page.SetCompleted();

        PageModel? currentPage = section.Pages.FindPage(page.Path);
        
        // Swap the page in the section (if it exists)
        section.Pages.SwapPage(page);
        
        IPageExecutor executor = _serviceProvider.GetKeyedService<IPageExecutor>(node.FlowNodeId) ?? NoopPageExecutor.Default;
        ExecutePageContext context = new() { Form = form, Section = section, CurrentPage = page, PageBeforeChanges = currentPage };
        await executor.ExecuteAsync(context);

        TreeNode nextNode = node.Children[context.ChildNodeIndex];
        section.TreeNodeId = nextNode.Id;
        
        PageModel? nextPage = section.Pages.FindPage(nextNode.PagePath);

        // If we find a page and the next node Id matches then we are not changing branch and we may be going back to a specific page
        if (nextPage is not null && nextPage.LinkToTreeNode == nextNode.Id)
        {
            return section.ReturnUrl ?? nextNode.PagePath;
        }
        
        // Clear all downstream children
        foreach (TreeNode descendant in node.GetAllDescendants())
        {
            PageModel? descendantPage = section.Pages.FindPage(descendant.PagePath);

            if (descendantPage is not null && descendantPage.LinkToTreeNode == descendant.Id)
            {
                section.Pages.Remove(descendantPage);
            }
        }
        
        return nextNode.PagePath;
    }

    private async ValueTask RunLoaderAsync(
        FormModel form, 
        SectionModel section, 
        PageModel currentPage, 
        TreeNode pageTreeNode, 
        Dictionary<string, string?> queryParams)
    {
        IPageLoader loader = _serviceProvider.GetKeyedService<IPageLoader>(pageTreeNode.FlowNodeId) ?? NoopPageLoader.Default;
        LoadPageContext context = new() { Form = form, Section = section, CurrentPage = currentPage, QueryParams = queryParams };
        await loader.LoadAsync(context);
    }

    private static PageModel CreatePage(SectionModel section, TreeNode pageNode)
    {
        PageModel page = section.Pages.FindPage(pageNode.PagePath) ?? (PageModel)Activator.CreateInstance(pageNode.PageType, [])!;
        page.Path = pageNode.PagePath;
        page.MetaData2 = pageNode.MetaData;
        page.LinkToTreeNode = pageNode.Id;
        return page;
    }
}