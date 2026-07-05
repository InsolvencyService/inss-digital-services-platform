using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.MetaData;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.PageFlow;

public interface ITreeViewManager
{
    ContentPath TransitionToStart(SectionModel section);
    ValueTask<PageModel> LoadAsync(FormModel form, SectionModel section, ContentPath path, Dictionary<string, string?> queryParams);
    ValueTask<ValidationResult[]> ValidateAsync(FormModel form, SectionModel section, PageModel page);
    ValueTask<ContentPath> SaveAsync(FormModel form, SectionModel section, PageModel page);
}

public class TreeViewManager : ITreeViewManager
{
    private readonly ITreeNodeFactory _treeNodeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TreeViewManager> _logger;

    public TreeViewManager(ITreeNodeFactory treeNodeFactory, IServiceProvider serviceProvider, ILogger<TreeViewManager> logger)
    {
        _treeNodeFactory = treeNodeFactory;
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
        
        IPagePropertiesProvider pagePropertiesProvider = _serviceProvider.GetRequiredService<IPagePropertiesProvider>();
        TreeNode rootNode = _treeNodeFactory.GetRootNode(section.Path);
        TreeNode pageNode = rootNode.GetNode(section.TreeNodeId);

        // Handle back button navigation
        if (pageNode.PagePath != path)
        {
            TreeNode x = rootNode.FindNodeForPath(path)!; // TODO Fix ! Perhaps throw as an invalid action?
            
            PageModel parentPage = section.Pages.FindPage(x.PagePath) ?? (PageModel)Activator.CreateInstance(x.PageType, [])!;
            parentPage.Path = x.PagePath;
            parentPage.MetaData2 = x.MetaData;
            section.TreeNodeId = x.Id;

            IPageLoader loader2 = _serviceProvider.GetKeyedService<IPageLoader>(x.FlowNodeId) ?? NoopPageLoader.Default;
            LoadPageContext context2 = new() { Form = form, Section = section, CurrentPage = parentPage, QueryParams = queryParams };
            await loader2.LoadAsync(context2);
            
            if (section.ReturnUrl is not null)
            {
                pagePropertiesProvider.PreviousPagePath = section.ReturnUrl;
            }
            else
            {
                TreeNode? parentNode2 = rootNode.FindParent(x);
                pagePropertiesProvider.PreviousPagePath = parentNode2?.PagePath ?? "/";
            }

            return parentPage;
        }
        
        PageModel page = (PageModel)Activator.CreateInstance(pageNode.PageType, [])!;
        page.Path = pageNode.PagePath;
        page.MetaData2 = pageNode.MetaData;
        
        IPageLoader loader = _serviceProvider.GetKeyedService<IPageLoader>(pageNode.FlowNodeId) ?? NoopPageLoader.Default;
        LoadPageContext context = new() { Form = form, Section = section, CurrentPage = page, QueryParams = queryParams };
        await loader.LoadAsync(context);

        TreeNode? parentNode = rootNode.FindParent(pageNode);
        pagePropertiesProvider.PreviousPagePath = parentNode?.PagePath ?? "/";
        return page;
/*        
        // If the section has no pages then we need to add a new page and assign the Id to it
        
        

        PageModel? page = section.Pages.FindPage(path);

        if (page is null)
        {
            page = (PageModel)Activator.CreateInstance(rootNode.PageType, [])!;
            page.Path = rootNode.PagePath;
            page.TreeNodeId = rootNode.Id;
            section.Pages.Add(page);
        }
        
        TreeNode node = rootNode.GetNode(page.TreeNodeId);
        
        //TreeNode pageTreeNode = rootNode.FindNodeForPath(path) ?? rootNode;
        //section.TreeNodeId ??= rootNode.Id;

        TreeNode? node = rootNode.FindNode(section.TreeNodeId);

        if (node is null)
        {
            throw new InvalidOperationException($"Unable to find the tree node for {section.TreeNodeId}."); // TODO: Fix
        }

        if (path != form.Path && node.PagePath != path)
        {
            // Is it the child node
            TreeNode? childNode = node.Children.FirstOrDefault(n => n.PagePath == path);

            if (childNode is null)
            {
                // Is the parent (back button)
                TreeNode? parentNode2 = rootNode.FindParent(node);

                if (parentNode2 is null)
                {
                    throw new InvalidOperationException("Did not find the child!"); // TODO: Fix
                }

                section.TreeNodeId = parentNode2.Id;
                node = parentNode2;
            }
            else
            {
                section.TreeNodeId = childNode.Id;
                node = childNode;
            }
        }

        // if (node.Node.PagePath != path)
        // {
        //     throw new InvalidOperationException("Mismatch between page requested and node."); // TODO: Fix
        // }
        
        // Does the page exist for the node?
        PageModel? page = section.Pages.FindPage(path);

        if (page is null)
        {
            // Create an instance of the page
            // Set the metadata
            // Add to section
            page = (PageModel)Activator.CreateInstance(node.PageType, [])!;
            page.Path = node.PagePath;
            section.Pages.Add(page);
        }
        
        // Set metadata
        page.MetaData2 = node.MetaData;
        
        // Find the loader and execute it
        IPageLoader loader = _serviceProvider.GetKeyedService<IPageLoader>(node.FlowNodeId) ?? NoopPageLoader.Default;
        LoadPageContext context = new() { Form = form, Section = section, CurrentPage = page, QueryParams = queryParams };
        await loader.LoadAsync(context);

        IPagePropertiesProvider pagePropertiesProvider = _serviceProvider.GetRequiredService<IPagePropertiesProvider>();
        TreeNode? parentNode = rootNode.FindParent(node);
        pagePropertiesProvider.PreviousPagePath = parentNode?.PagePath ?? "/";
        return page;
        */
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
        
        // TODO: Temp - just get first child
        await Task.Delay(10);

        
        
        
        IPageExecutor executor = _serviceProvider.GetKeyedService<IPageExecutor>(node.FlowNodeId) ?? NoopPageExecutor.Default;
        ExecutePageContext context = new()
        {
            //Nodes = Nodes, 
            //CurrentNode = node,
            //CurrentNode2 = node,
            Form = form, 
            Section = section, 
            CurrentPage = page, 
            PageBeforeChanges = currentPage
        };
        await executor.ExecuteAsync(context);

        TreeNode nextNode = node.Children[context.ChildNodeIndex];
        section.TreeNodeId = nextNode.Id;
        return nextNode.PagePath;
        
        /*PageModel targetPage = section.Pages.GetPage(page.Path);
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

        return nextPagePath;*/
    }
}

public sealed class TreeNode
{
    private readonly FlowNode _flowNode;

    public TreeNode(FlowNode flowNode)
    {
        _flowNode = flowNode;
    }
    
    public string Id { get; init; } = Guid.NewGuid().ToString(); // TODO: Primitive

    public NodeId FlowNodeId => _flowNode.Id;
    
    public Type PageType => _flowNode.PageType;
    
    public ContentPath PagePath => _flowNode.PagePath;
    
    public PageMetaDataList MetaData => _flowNode.MetaData;
    
    public TreeNode[] Children { get; set; } = [];

    public TreeNode? FindNode(string id)
    {
        if (Id == id)
        {
            return this;
        }

        foreach (TreeNode childNode in Children)
        {
            TreeNode? match = childNode.FindNode(id);

            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    public TreeNode GetNode(string id)
    {
        return FindNode(id) ?? throw new InvalidOperationException($"Unable to find the tree node for {id}.");
    }

    public TreeNode? FindNodeForPath(ContentPath path)
    {
        if (PagePath == path)
        {
            return this;
        }

        foreach (TreeNode childNode in Children)
        {
            TreeNode? matchedNode = childNode.FindNodeForPath(path);

            if (matchedNode is not null)
            {
                return matchedNode;
            }
        }

        return null;
    }
    
    public TreeNode? FindParent(TreeNode node)
    {
        return node.Id == Id ? null : FindParent(this, node);
    }

    private static TreeNode? FindParent(TreeNode parentNode, TreeNode nodeToMatch)
    {
        foreach (TreeNode childNode in parentNode.Children)
        {
            if (childNode.Id == nodeToMatch.Id)
            {
                return parentNode;
            }

            TreeNode? matchedParentNode = FindParent(childNode, nodeToMatch);

            if (matchedParentNode is not null)
            {
                return matchedParentNode;
            }
        }

        return null;
    }
}

public interface ITreeNodeFactory
{
    TreeNode GetRootNode(ContentPath sectionPath);
}
public sealed class TreeNodeFactory : ITreeNodeFactory
{
    private readonly Dictionary<ContentPath, TreeNode> _sectionRootNodes = [];
    
    public TreeNode GetRootNode(ContentPath sectionPath)
    {
        return _sectionRootNodes.TryGetValue(sectionPath, out TreeNode? rootNode) ? rootNode : throw new InvalidOperationException("??"); // TODO: Fix exception
    }
    
    public void Add(TreeNode rootNode, ContentPath sectionPath)
    {
        _sectionRootNodes[sectionPath] = rootNode;
    }
}