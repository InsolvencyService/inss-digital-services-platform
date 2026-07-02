using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.MetaData;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Components.Builders;

public class FormBuilder
{
    private readonly FormModel _form;
    private readonly IServiceCollection _services;
    private readonly TreeNodeFactory _treeNodeFactory;
    private readonly FormPathManager _formPathManager;
    
    private FormBuilder(IServiceCollection services, string path, SubmitTypes submitType)
    {
        _form = new FormModel { Path = $"/{path}", SubmitType = submitType};
        _services = services;
        _treeNodeFactory = new TreeNodeFactory();
        _formPathManager = new FormPathManager();
        _formPathManager.AddPath(_form.Path);
    }
    
    public static FormBuilder Create(IServiceCollection services, string path, SubmitTypes submitType = SubmitTypes.Form)
    {
        return new FormBuilder(services, path, submitType);
    }

    public SectionBuilder AddSection(string title, string path)
    {
        return new SectionBuilder(_services, this, _treeNodeFactory, _formPathManager, _form, title, path);
    }

    public void FinalizeForm()
    {
        _form.Validate();
        _services.AddTransient<IFormFactory>(_ => new FormFactory(_form));
        _services.AddTransient<ITreeViewManager, TreeViewManager>();
        _services.AddSingleton<ITreeNodeFactory>(_ => _treeNodeFactory);
        _services.AddSingleton<IFormPathManager>(_ => _formPathManager);
    }
}

public sealed class SectionBuilder
{
    private readonly FormBuilder _formBuilder;
    private readonly TreeNodeFactory _treeNodeFactory;
    private readonly FormPathManager _formPathManager;
    private readonly FormModel _form;
    private readonly SectionModel _section;
    private readonly List<FlowNode> _nodes = [];
    private readonly IServiceCollection _services;
    
    internal SectionBuilder(IServiceCollection services, FormBuilder formBuilder, TreeNodeFactory treeNodeFactory, FormPathManager formPathManager, FormModel form, string title, string path)
    {
        _formBuilder = formBuilder;
        _treeNodeFactory = treeNodeFactory;
        _formPathManager = formPathManager;
        _services = services;
        _form = form;
        _section = new SectionModel { Title = title, Path = $"{form.Path}/{path}", SubmitType = _form.SubmitType };
        _form.Sections.Add(_section);
    }
    
    public FlowNodeBuilder AddNode<TPageModel>(NodeId nodeId, ContentPath pagePath, NodeId[] nextNodeIds) where TPageModel : PageModel
    {
        FlowNode node = new() { Id = nodeId, PagePath = $"{_section.Path}/{pagePath}", PageType = typeof(TPageModel), NextNodes = nextNodeIds };
        _nodes.Add(node);
        _formPathManager.AddPath(node.PagePath);
        return new FlowNodeBuilder(_formBuilder, this, node, _services);
    }

    public FormBuilder RegisterSection()
    {
        _treeNodeFactory.Add(AsTree(), _section.Path);
        return _formBuilder;
    }
    
    private TreeNode AsTree()
    {
        Dictionary<NodeId, FlowNode> lookup = _nodes.ToDictionary(n => n.Id);
        return CreateTreeNode(_nodes[0], lookup);
    }
        
    private static TreeNode CreateTreeNode(FlowNode node, Dictionary<NodeId, FlowNode> lookup)
    {
        TreeNode treeNode = new(node);

        foreach (NodeId nextNodeId in node.NextNodes)
        {
            if (lookup.TryGetValue(nextNodeId, out var childNode))
            {
                List<TreeNode> existingChildren = treeNode.Children.ToList();
                existingChildren.Add(CreateTreeNode(childNode, lookup));
                treeNode.Children = existingChildren.ToArray();
            }
            else
            {
                throw new InvalidOperationException($"Missing {nextNodeId}");
            }
        }

        return treeNode;
    }
}

public sealed class FlowNodeBuilder
{
    private readonly FormBuilder _formBuilder;
    private readonly SectionBuilder _sectionBuilder;
    private readonly FlowNode _node;
    private readonly IServiceCollection _services;

    internal FlowNodeBuilder(FormBuilder formBuilder, SectionBuilder sectionBuilder, FlowNode node, IServiceCollection services)
    {
        _formBuilder = formBuilder;
        _sectionBuilder = sectionBuilder;
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
    
    public FlowNodeBuilder WithMetaData(PageMetaData2 metaData)
    {
        _node.AddMetaData(metaData);
        return this;
    }
    
    // TODO: Page metadata
    
    public SectionBuilder NextNode()
    {
        return _sectionBuilder;
    }

    public SectionBuilder NodesDone()
    {
        return _sectionBuilder;
    }
}

public interface IFormPathManager
{
    ContentPath[] Paths { get; }
}
public sealed class FormPathManager : IFormPathManager
{
    private readonly List<ContentPath> _paths = [];

    public ContentPath[] Paths => _paths.ToArray();
    
    public void AddPath(ContentPath path)
    {
        if (!_paths.Contains(path))
        {
            _paths.Add(path);
        }
    }
}
