using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public sealed class JourneyService : IJourneyService
{
    private readonly IServiceProvider _serviceProvider;

    public JourneyService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void TransitionPrevious(FormModel form, PageModel? pageModel = null)
    {
        if (pageModel is not null)
        {
            var previousNavigation = form.NavigationHistory.Last();

            // If we have navigated back and we are on the page of the last entry then we need to remove it
            if (previousNavigation == pageModel.PageUrl)
            {
                form.PopLastNavigationHistory();
            }
            
            pageModel.PreviousPageUrl = form.NavigationHistory.Last();
        }
    }
    
    public void TransitionNext(FormModel form, PageModel pageModel)
    {
        var resolver = GetJourneyResolver(pageModel);
            
        var nextPage = resolver.Resolve(form, pageModel);

        if (nextPage is not null)
        {
            pageModel.NextPageUrl = nextPage.PageUrl;
            return;
        }
        
        var section = form.FindSectionForPage(pageModel.PageUrl);

        if (section.IsLastPageInSection(pageModel))
        {
            section.PreviousPageUrl = pageModel.PageUrl;
            pageModel.NextPageUrl = section.PageUrl;
        }
        else
        {
            pageModel.NextPageUrl = $"/{form.PageUrl}";
        }
    }
    
    private IJourneyResolver GetJourneyResolver(PageModel page)
    {
        var resolverType = typeof(IJourneyResolver<>).MakeGenericType(page.GetType());
        return (IJourneyResolver)(_serviceProvider.GetService(resolverType) ?? new DefaultJourneyResolver());
    }
}