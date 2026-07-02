using System.Collections.Frozen;
using GovUk.Forms.Domain.Exceptions;

namespace GovUk.Forms.Domain.MetaData;

public sealed class PageMetaDataList : List<PageMetaData2>
{
    public PageMetaData2? Find(string tag)
    {
        return this.FirstOrDefault(m => m.Tag == tag);
    }
    
    public PageMetaData2? FindButton()
    {
        return this.FirstOrDefault(m => m.Tag == "Button");
    }
    
    public PageMetaData2 Get(string tag)
    {
        return this.FirstOrDefault(m => m.Tag == tag) ?? throw new ModelException($"Unable to find the meta data for tag {tag}");
    }

    public PageMetaData2 GetButton()
    {
        return Get("Button");
    }
    
    public PageMetaData2 GetQuestion()
    {
        return Get("Question");
    }
    
    public PageMetaData2 GetHint()
    {
        return Get("Hint");
    }
}