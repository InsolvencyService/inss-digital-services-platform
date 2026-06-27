using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public class SectionModel : ContentModel
{
    public string Title { get; init; } = string.Empty;
    
    public PageModelList Pages { get; init; } = [];

    public DateTimeOffset? StartedDate { get; set; }
    
    public DateTimeOffset? CompletedDate { get; set; }

    public NodeId[] VisitedNodes { get; set; } = [];
    
    public ContentPath? PreviousPagePath { get; set; }
    
    [JsonIgnore]
    public PageModel FirstPage
    {
        get
        {
            if (CompletedDate is not null)
            {
                return Pages.GetFirstOf<SummaryModel>();
            }
            
            return Pages[0] is GroupPageModel groupPage ? groupPage.Pages[0] : Pages[0];
        }
    }
    
    public void SetInProgress()
    {
        StartedDate ??= DateTimeOffset.Now;
    }

    public void SetCompleted()
    {
        CompletedDate = DateTimeOffset.Now;
    }
    
    public void Track(NodeId? nodeId)
    {
        if (nodeId is not null && VisitedNodes.IndexOf(nodeId) == -1)
        {
            List<NodeId> nodeIdList = [..VisitedNodes, nodeId];
            VisitedNodes = nodeIdList.ToArray();
        }
    }

    public void Untrack(params NodeId[] nodeIdToUntrack)
    {
        List<NodeId> nodeIdList = [..VisitedNodes];

        foreach (NodeId nodeId in nodeIdToUntrack)
        {
            nodeIdList.Remove(nodeId);
        }
        
        VisitedNodes = nodeIdList.ToArray();
    }
    
    public void ResetVisitedNodesFrom(NodeId? fromNodeId)
    {
        int currentPageNodeIndex = VisitedNodes.IndexOf(fromNodeId);

        if (currentPageNodeIndex > -1)
        {
            NodeId[] nodeIdsToReset = VisitedNodes.Skip(currentPageNodeIndex + 1).ToArray();
            IEnumerable<PageModel> pagesToReset = nodeIdsToReset.Select(nodeId => Pages.First(p => p.LinkedToNode == nodeId));

            foreach (PageModel resetPage in pagesToReset)
            {
                resetPage.ClearValues();
            }
                    
            Untrack(nodeIdsToReset);
        }
    }
}