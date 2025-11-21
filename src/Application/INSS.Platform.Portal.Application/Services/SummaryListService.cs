using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public class SummaryListService : BasePageModelService<SummaryListModel>
{
    private readonly IFormStateService _formStateService;
    private readonly IJourneyService _journeyService;
    private readonly IUserSessionResolver _userSessionResolver;

    public SummaryListService(
        IFormStateService formStateService,
        IJourneyService journeyService,
        IUserSessionResolver userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
        _formStateService = formStateService;
        _journeyService = journeyService;
        _userSessionResolver = userSessionResolver;
    }

    public override async Task<string> GetPageUrlAsync(string? pageUrl, string id)
    { 
        Console.WriteLine($"GetPageUrlAsync called with id: {_formStateService} {pageUrl}");

        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        SummaryListModel page = form.FindPage<SummaryListModel>(pageUrl!);
        PageModel pageToChange = page.Pages.First(p => p.Id == id);

        SectionModel section = form.FindSectionForPage(page.PageUrl);
        PageModel? previousPage = section.GetPreviousPage(page.PageUrl);
        pageToChange.CopyTo(previousPage!);

        return await Task.FromResult(pageToChange.PageUrl); 
    }

    protected override void CopySourceToTargetModel(SummaryListModel sourceModel, SummaryListModel targetModel)
    {
        //targetModel.Pages.AddRange(sourceModel.SummaryList ?? Enumerable.Empty<AddressModel>());
    }

    public override async Task<SummaryListModel> LoadAsync(string? pageUrl)
    { 
        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        SummaryListModel page = form.FindPage<SummaryListModel>(pageUrl!);
        _journeyService.TransitionPrevious(form, page);
        form.AddOrUpdatePreviousPageInSummaryList(page);

        await _formStateService.SaveAsync(_userSessionResolver.GetUserId(), form);
        return page;
    }
}