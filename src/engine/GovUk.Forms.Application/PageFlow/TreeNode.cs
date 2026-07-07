using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain.MetaData;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.PageFlow;

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

    public TreeNode[] GetAllDescendants()
    {
        List<TreeNode> descendants = [];

        foreach (TreeNode childNode in Children)
        {
            descendants.Add(childNode);

            TreeNode[] childDescendants = childNode.GetAllDescendants();
            descendants.AddRange(childDescendants);
        }
        
        return descendants.ToArray();
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