using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Resolvers;

public sealed class DefaultJourneyResolver : IJourneyResolver
{
    public PageModel? Resolve(FormModel form, PageModel pageModel)
    {
        SectionModel section = form.FindSectionForPage(pageModel.PageUrl);
        return section.GetNextPage(pageModel.PageUrl);
    }
}

public sealed class SummaryListJourneyResolver : IJourneyResolver<SummaryListModel>
{
    public PageModel? Resolve(FormModel form, PageModel pageModel)
    {
        if (pageModel is SummaryListModel summaryList)
        {
            // We know the next page is the confirm page, so will skip it

            SectionModel section = form.FindSectionForPage(pageModel.PageUrl);

            PageModel? nextPage = section.GetNextPage(summaryList.PageUrl)!;

            return section.GetNextPage(nextPage.PageUrl);
        }

        return null; // section.GetNextPage(pageModel.PageUrl);
    }
}