using GovUk.Forms.Domain.MetaData;

namespace GovUk.Forms.Components.Resolvers;

public interface IPageMetaDataResolver
{
    ValueTask<string> ResolveAsync(PageMetaData2 metaData);
}