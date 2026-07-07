using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.PageFlow;

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