using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Resolvers;

public sealed class DefaultJourneyResolver : IJourneyResolver
{
    public PageModel? Resolve(FormModel form, PageModel pageModel)
    {
        var section = form.FindSectionForPage(pageModel.PageUrl);
        return section.GetNextPage(pageModel.PageUrl);
    }
}