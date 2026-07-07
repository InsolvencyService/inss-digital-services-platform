using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.PageFlow;

public interface ITreeNodeFactory
{
    TreeNode GetRootNode(ContentPath sectionPath);
}