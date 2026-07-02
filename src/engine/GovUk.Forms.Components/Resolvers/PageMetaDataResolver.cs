using GovUk.Forms.Components.Exceptions;
using GovUk.Forms.Domain.MetaData;

namespace GovUk.Forms.Components.Resolvers;

public sealed class PageMetaDataResolver : IPageMetaDataResolver
{
    public ValueTask<string> ResolveAsync(PageMetaData2 metaData)
    {
        if (metaData.Text is not null)
        {
            return ValueTask.FromResult(metaData.Text);
        }

        throw new ComponentException($"Currently do not support key-based meta data. Tag is {metaData.Tag}.");
    }
}