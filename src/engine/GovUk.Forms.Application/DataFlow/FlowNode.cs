using GovUk.Forms.Domain.MetaData;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow;

public sealed class FlowNode
{
    private readonly PageMetaDataList _metaDataList = [];
    
    public NodeId Id { get; init; }
    
    public required ContentPath PagePath { get; init; }

    public NodeId[] NextNodes { get; init; } = [];
    
    public Type PageType { get; init; }

    public PageMetaDataList MetaData => _metaDataList; // TODO: Sort this
    
    public void AddMetaData(PageMetaData2 metaData)
    {
        if (!_metaDataList.Contains(metaData))
        {
            _metaDataList.Add(metaData);
        }
    }
}